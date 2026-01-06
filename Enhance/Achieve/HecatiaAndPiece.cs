using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class HecatiaAndPiece : BaseEnhance
    {
        public override string Text => GetText("HecatiaAndPiece");
        public override string[] ExperimentalText => [GetText("HecatiaAndPiece_1")];
        public override bool[] Experimental => [Config.Hecatia];
        public override bool EnableRightClick => false;
        public override bool Passive => true;

        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<HecatiaPlanet>());
        }

        public override void PlayerResetEffects(Player player)
        {
            if (Main.hardMode)
                player.MP().ActiveEnhanceCount++;

            if (NPC.downedMoonlord)
                player.MP().ActiveEnhanceCount++;
        }

        public override void NPCAI(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || npc.dontTakeDamage || npc.friendly ||
                NPCID.Sets.CountsAsCritter[npc.type])
                return;

            if (EnhanceCount == null)
                return;

            if (!EnhanceRegistry.TryGetEnhanceId(ModContent.ItemType<HecatiaPlanet>(), out EnhancementId enhanceId))
                return;

            EnhanceCount.TryGetValue(enhanceId, out int magnification);

            // 说明：火把统计本身是“重活”（大范围 tile 遍历），不能放在 NPCAI 里直接跑。
            // 做法：把扫描拆成“跨多帧推进”的迭代器任务，并缓存扫描结果（TorchBuckets=ceil(torchTiles/10)）。
            // NPCAI 每 tick 只读取缓存并做 O(1) 加法；缓存可能延迟更新，但本能力允许延迟生效。
            int torchBuckets = HecatiaTorchScanSystem.GetTorchBuckets(npc, enhanceId);
            if (magnification > 0 && torchBuckets > 0)
                npc.GetGlobalNPC<GEnhanceNPCs>().TorchDamage += torchBuckets * magnification;

            if (Main.GameUpdateCount % 30 == 17 && npc.GetGlobalNPC<GEnhanceNPCs>().TorchDamage > 0)
            {
                npc.SimpleStrikeNPC(npc.GetGlobalNPC<GEnhanceNPCs>().TorchDamage, 0);
                npc.GetGlobalNPC<GEnhanceNPCs>().TorchDamage = 0;
            }
        }
    }

    /// <summary>
    /// 赫卡提亚火把扫描调度器：用迭代器把大范围 tile 扫描拆分到多帧，并用全局预算限制每帧工作量。
    /// <para>只在服务端/单机运行，客户端不执行世界伤害计算。</para>
    /// </summary>
    public sealed class HecatiaTorchScanSystem : ModSystem
    {
        // 可调参数：越大越“准确/更新快”，但每帧占用越多；越小越“省”，但延迟更明显。
        private const int TorchScanRadiusTiles = 225;

        // 刷新间隔：每个 NPC 的火把缓存多久重算一次（tick）。
        private const int TorchScanRefreshTicks = 60;

        // 全局预算：每帧最多处理多少个 tile（所有 NPC 合计）。
        private const int TorchScanTilesBudgetPerTick = 15000;

        // 迭代器让出控制权的粒度：每处理多少个 tile yield 一次。
        private const int TorchScanChunkSize = 1024;

        private sealed class TorchCache
        {
            // TorchBuckets：ceil(torchTiles/10)，对应原先 damage/10f 的统计方式。
            public int TorchBuckets;

            // 下次允许刷新缓存的 tick（避免每帧都重新发起扫描）。
            public uint NextRefreshTick;

            // 正在进行的扫描任务；为 null 表示当前没有进行中的扫描。
            public TorchScanJob Job;

            // ActiveNpcIds 中的位置（用于 O(1) swap-remove）。-1 表示不在活跃列表里。
            public int ActiveIndex = -1;
        }

        /// <summary>
        /// 遍历火把的工作类，通过这个迭代内容
        /// </summary>
        private sealed class TorchScanJob
        {
            public int NpcId;
            public int CenterTileX;
            public int CenterTileY;
            public int MinX;
            public int MaxX;
            public int MinY;
            public int MaxY;
            public int RadiusSq;

            public int TorchTiles;

            // 迭代器状态机：跨帧保存进度。每次 MoveNext 会推进一小段扫描。
            public IEnumerator<int> Enumerator;

            public int TorchBuckets => (TorchTiles + 9) / 10;

            public bool Step(ref int budget)
            {
                // Lazy 初始化迭代器，避免创建无用状态机。
                Enumerator ??= ScanTorchTiles(this);

                // 通过MoveNext检查迭代器是否有返回值？如果有返回值那么就会处理伤害
                if (!Enumerator.MoveNext())
                    return false;

                // Enumerator.Current 表示“本次推进处理了多少个 tile”，用于扣减全局预算。
                budget -= Enumerator.Current;

                return true;
            }
        }

        private static TorchCache[] CacheByNpcId;

        // 只维护“当前有扫描任务”的 npcId，避免每帧空转遍历 Main.maxNPCs。
        private static List<int> ActiveNpcIds;
        private static int ActiveCursor;

        public override void Load()
        {
            CacheByNpcId = null;
            ActiveNpcIds = new List<int>(64);
            ActiveCursor = 0;
        }

        public override void Unload()
        {
            CacheByNpcId = null;
            ActiveNpcIds = null;
            ActiveCursor = 0;
        }

        public override void PostUpdateNPCs()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // tML 会每帧调用 PostUpdateNPCs；如果没人启用/当前没有任务，就直接早退。
            if (ActiveNpcIds == null || ActiveNpcIds.Count == 0)
                return;

            EnsureCacheArray();

            // Round-robin 推进：避免总是让某些 NPC 的任务先/后完成。
            int budget = TorchScanTilesBudgetPerTick;

            if (budget <= 0)
                return;

            // 注意：ActiveNpcIds 的长度会随着任务完成/取消而变化，因此每轮都基于当前 Count 取模。
            int loops = ActiveNpcIds.Count;
            for (int iter = 0; iter < loops && budget > 0 && ActiveNpcIds.Count > 0; iter++)
            {
                int index = ActiveCursor++ % ActiveNpcIds.Count;
                int npcId = ActiveNpcIds[index];
                TorchCache cache = CacheByNpcId[npcId];

                // 兜底：活跃列表里应当都有任务；若出现不同步，直接移除。
                TorchScanJob job = cache?.Job;
                if (job == null)
                {
                    if (cache != null)
                        RemoveActiveJob(cache, npcId);

                    continue;
                }

                // NPC 已失活/不可被伤害时直接取消任务，避免无意义扫描。
                NPC npc = Main.npc[npcId];
                if (!npc.active || npc.dontTakeDamage || npc.friendly || NPCID.Sets.CountsAsCritter[npc.type])
                {
                    cache.Job = null;
                    RemoveActiveJob(cache, npcId);

                    continue;
                }

                if (job.Step(ref budget))
                    continue;

                // 扫描完成：更新缓存并清掉 job（下次刷新由 NextRefreshTick 控制）。
                cache.TorchBuckets = job.TorchBuckets;
                cache.NextRefreshTick = (uint)Main.GameUpdateCount + TorchScanRefreshTicks;
                cache.Job = null;
                RemoveActiveJob(cache, npcId);
            }
        }

        public static int GetTorchBuckets(NPC npc, EnhancementId enhanceId)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return 0;

            if (EnhanceCount == null)
                return 0;

            // 没有人启用时不需要更新/应用（但旧缓存仍可保留）。
            if (!EnhanceCount.TryGetValue(enhanceId, out int magnification) || magnification <= 0)
                return 0;

            EnsureCacheArray();

            int npcId = npc.whoAmI;
            TorchCache cache = CacheByNpcId[npcId] ??= new TorchCache();

            uint now = (uint)Main.GameUpdateCount;
            if (cache.Job == null && (cache.NextRefreshTick == 0 || now >= cache.NextRefreshTick))
            {
                // 发起一次新的扫描任务：只记录必要的快照参数（中心 tile、范围、半径平方）。
                cache.Job = CreateJob(npc, npcId, TorchScanRadiusTiles);
                AddActiveJob(cache, npcId);
            }

            return cache.TorchBuckets;
        }

        private static void EnsureCacheArray()
        {
            if (CacheByNpcId == null || CacheByNpcId.Length != Main.maxNPCs)
            {
                CacheByNpcId = new TorchCache[Main.maxNPCs];
                ActiveNpcIds?.Clear();
                ActiveCursor = 0;
            }
        }

        private static void AddActiveJob(TorchCache cache, int npcId)
        {
            if (ActiveNpcIds == null)
                return;

            if (cache.ActiveIndex != -1)
                return;

            cache.ActiveIndex = ActiveNpcIds.Count;
            ActiveNpcIds.Add(npcId);
        }

        private static void RemoveActiveJob(TorchCache cache, int npcId)
        {
            if (ActiveNpcIds == null)
                return;

            int index = cache.ActiveIndex;

            if (index < 0)
                return;

            int lastIndex = ActiveNpcIds.Count - 1;
            int lastNpcId = ActiveNpcIds[lastIndex];

            // swap-remove：把最后一个挪到被删除的位置，再删掉尾部。
            ActiveNpcIds[index] = lastNpcId;
            ActiveNpcIds.RemoveAt(lastIndex);

            cache.ActiveIndex = -1;

            if (lastNpcId != npcId)
            {
                TorchCache lastCache = CacheByNpcId[lastNpcId];
                if (lastCache != null)
                    lastCache.ActiveIndex = index;
            }
        }

        private static TorchScanJob CreateJob(NPC npc, int npcId, int radiusTiles)
        {
            // 注意：这里统一用 tile 坐标计算距离，避免每格都做浮点除法。
            int centerTileX = (int)(npc.Center.X / 16f);
            int centerTileY = (int)(npc.Center.Y / 16f);

            int minX = centerTileX - radiusTiles;
            int maxX = centerTileX + radiusTiles;
            int minY = centerTileY - radiusTiles;
            int maxY = centerTileY + radiusTiles;

            ClampTilePos(ref minX, ref maxX, ref minY, ref maxY);

            return new TorchScanJob
            {
                NpcId = npcId,
                CenterTileX = centerTileX,
                CenterTileY = centerTileY,
                MinX = minX,
                MaxX = maxX,
                MinY = minY,
                MaxY = maxY,
                RadiusSq = radiusTiles * radiusTiles,
                TorchTiles = 0,
                Enumerator = null
            };
        }

        private static void ClampTilePos(ref int minTileX, ref int maxTileX, ref int minTileY, ref int maxTileY)
        {
            // tile index 范围为 [0, maxTilesX/Y - 1]
            minTileX = Math.Max(0, minTileX);
            maxTileX = Math.Min(Main.maxTilesX - 1, maxTileX);
            minTileY = Math.Max(0, minTileY);
            maxTileY = Math.Min(Main.maxTilesY - 1, maxTileY);
        }

        /// <summary>
        /// 通过迭代器实现分段处理NPC通过火把给自己造成伤害的逻辑
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private static IEnumerator<int> ScanTorchTiles(TorchScanJob job)
        {
            // 迭代器约定：每处理 TorchScanChunkSize 个 tile yield 一次，让出控制权（实现“分帧”）。

            // 谢谢GPT5.2 high 让我了解了迭代器，让我本人来说明一下下面的代码干啥了
            // 在低级别C#代码里面，编译器把这个迭代器的方法编译为一个有State和目前值的状态机的class。
            // 然后接下来！就是很抽象的玩意了
            // 因为迭代器的原因，这些局部变量实际都被缓存了，代码在Rider的低级别C#显示是：
            /*
                private int <>1__state;
                private int <>2__current;
                public HecatiaTorchScanSystem.TorchScanJob job;
                private int <processed>5__1;
                private int <x>5__2;
                private int <dx>5__3;
                private int <dx2>5__4;
                private int <y>5__5;
                private int <dy>5__6;
                private Tile <tile>5__7;
             */
            // 而接下来我会用 DP： 来说明每一步干了什么

            // DP: 这里其实是初始化了一个叫进度的int值，ScanTorchTiles只在迭代器被创建那时候调用这一段，所以它就是个初始化。
            int processed = 0;
            // DP: 如果你们有幸看到低级别C#代码，这里其实会被编译为goto（
            for (int x = job.MinX; x <= job.MaxX; x++)
            {
                // DP：这些局部变量也是迭代器的一部分！
                // 因为goto写法，所以在被编译后的C#代码部分中，这些内容其实哪里都能被访问，但是你实际写不行，因为这是编译器操作！
                // 而且因为被缓存...所以会有神奇的玩意，等会我会在 yield return那里解释
                int dx = x - job.CenterTileX;
                int dx2 = dx * dx;
                for (int y = job.MinY; y <= job.MaxY; y++)
                {
                    int dy = y - job.CenterTileY;

                    // 用平方距离与半径平方比较，避免开方；且与范围裁剪保持一致。
                    if (dx2 + dy * dy > job.RadiusSq)
                        continue;

                    Tile tile = Main.tile[x, y];
                    // Main.tile 是世界数据；这里只读 tileType，不做任何“写世界”的操作。
                    if (TileID.Sets.Torch[tile.TileType])
                        job.TorchTiles++;

                    // DP：最神仙的玩意来力
                    // 这里正常加啊，没有问题，然后下面的yield return...
                    processed++;
                    if (processed >= TorchScanChunkSize)
                    {
                        // DP：迭代器最抽象的东西
                        // 我解释一下具体干了啥：
                        // 首先IEnumerator.MoveNext()会调用这个方法，然后返回true或者false，表示是否还有下一个值。
                        // 在低级别C#代码里面，它是：
                        // 1. 修改当前状态 <>1__state 为 1
                        // 2. 修改当前值 <>2__current 为 <processed>5__1（也就是processed变量）
                        // 3. 返回true
                        yield return processed;
                        // DP：然后呢？上面返回true了欸，下次MoveNext会跑到这边开始循环
                        // 妈妈啊，它真的抽象。
                        processed = 0;
                    }
                }
            }

            // DP：同理，这里也是迭代器的一部分，被编译为goto
            // 迭代器返回false，表示没有下一个值了。
            // 这一段在低级别C#代码里面被编译为：
            /*
                if (this.<processed>5__1 > 0)
                  {
                        this.<>2__current = this.<processed>5__1;
                        this.<>1__state = 2;
                        return true;
                  }
             */
            if (processed > 0)
                yield return processed;
            // DP：迭代器最后在这里返回false，对的，下面什么都没有
        }
    }
}
