using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Items;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Core
{
    /// <summary>
    /// 增强系统的玩家侧核心状态（<see cref="ModPlayer"/>）。
    /// <para>
    /// 这里主要维护“玩家启用了哪些增强”（主动/被动），并在 Player 各生命周期钩子中分发增强逻辑。
    /// </para>
    /// </summary>
    public class EnhancePlayers : ModPlayer
    {
        public bool NewlyMadeDoll;
        public bool ABurntDoll;
        /// <summary>
        /// 主动启用的增强（通常由物品右键开关写入）。
        /// </summary>
        public List<EnhancementId> ActiveEnhance = [];
        /// <summary>
        /// 被动启用的增强（例如被动能力、宠物栏/照明栏等自动生效能力）。
        /// </summary>
        public List<EnhancementId> ActivePassiveEnhance = [];
        public int ActiveEnhanceCount = 11037;
        /// <summary>
        /// 萝莉丝用
        /// </summary>
        public int EatBook = 0;
        /// <summary>
        /// 大妖精用
        /// </summary>
        public int DaiyouseiCD = 0;
        /// <summary>
        /// 红美铃用，用于检测是否在落地时造成地震
        /// </summary>
        public bool Earthquake = false;
        /// <summary>
        /// 咲夜用，用于在关闭时将跳帧类型重置回去
        /// </summary>
        public Terraria.Enums.FrameSkipMode? frameSkipMode = null;
        /// <summary>
        /// 咲夜用
        /// </summary>
        public int SakuyaCD;
        /// <summary>
        /// 莉莉白用
        /// </summary>
        public int LilyCD = 0;
        /// <summary>
        /// 幽香四溢Buff用（风见幽香能力相关）
        /// </summary>
        public bool FragrantAromaFillsTheAir = false;
        /// <summary>
        /// 风见幽香-向阳花田用
        /// </summary>
        public int YukaCD = 0;
        /// <summary>
        /// 风见幽香-大南方丛林植物用
        /// </summary>
        public int SporeEruptionCD = 0;
        /// <summary>
        /// 蕾蒂用
        /// </summary>
        public int LettyCD = 0;
        /// <summary>
        /// 莉莉卡用
        /// </summary>
        public int LyricaCD = 0;
        /// <summary>
        /// 八云紫用
        /// </summary>
        public int YukariCD = 0;
        /// <summary>
        /// 灵梦-梦想天生用
        /// </summary>
        public bool ReimuCD = false;
        /// <summary>
        /// 莉格露用
        /// </summary>
        public int WriggleCD = 0;
        /// <summary>
        /// 米斯蒂娅用
        /// </summary>
        public int MystiaCD = 0;
        /// <summary>
        /// 上白泽慧音用，索引1记录的是上一个buff的type
        /// </summary>
        public int[] KeineCD = [0, -1];
        /// <summary>
        /// 铃仙用，记录闪避
        /// </summary>
        public bool ReisenDodge;
        /// <summary>
        /// 蓬莱山辉夜用
        /// </summary>
        public int[] OldBuff;
        /// <summary>
        /// 河城荷取用，记录原版合成站
        /// </summary>
        public bool[] adjTileVanilla;
        /// <summary>
        /// 河城荷取用，记录模组合成站
        /// </summary>
        public List<string> adjTileMod;
        /// <summary>
        /// 河城荷取用，记录全部合成站
        /// </summary>
        public bool[] adjTile;
        /// <summary>
        /// 河城荷取用，记录水、蜂蜜、岩浆、微光、炼药桌
        /// </summary>
        public bool[] adjOther;
        /// <summary>
        /// 东风谷早苗用
        /// </summary>
        public int SanaeCD;
        /// <summary>
        /// 火焰猫燐用
        /// </summary>
        public int RinCD;
        /// <summary>
        /// 古明地恋用，人气值
        /// </summary>
        public float Popularity;
        /// <summary>
        /// 姬虫百百世用
        /// <para>索引决定对应的加成：0—移动速度、1—挖矿速度、2—最大氧气值、3—最大生命值、4—岩浆免疫时间、5—伤害减免、6—暴击伤害、7/8/9—运气、10—百分比穿甲、11—防御效力</para>
        /// </summary>
        public int[] ExtraAddition = new int[12];
        /// <summary>
        /// 姬虫百百世用
        /// <para>索引决定对应的加成上限：0—移动速度、1—挖矿速度、2—最大氧气值、3—最大生命值、4—岩浆免疫时间、5—伤害减免、6—暴击伤害、7/8/9—运气、10—百分比穿甲、11—防御效力</para>
        /// </summary>
        public static int[] ExtraAdditionMax = [50, 50, int.MaxValue, 100, int.MaxValue, 50, 200, 10, 4, 1, 150, int.MaxValue];
        #region 防止闭包的私有字段们
        float ModifyLuck_luck;
        int GetHealLife_healValue;
        int GetHealMana_healValue;
        float ModifyItemScale_scale;
        float DrawEffects_r;
        float DrawEffects_g;
        float DrawEffects_b;
        float DrawEffects_a;
        bool DrawEffects_fullBright;
        Player.HurtModifiers ModifyHurt_modifiers;
        NPC.HitModifiers ModifyHitNPCWithItem_modifiers;
        NPC.HitModifiers ModifyHitNPCWithProj_modifiers;
        NPC.HitModifiers ModifyHitNPC_modifiers;
        bool PreKill_playSound;
        bool PreKill_genDust;
        PlayerDeathReason PreKill_damageSource;
        #endregion
        private static void ProcessDemonismAction(Player player, Action<BaseEnhance> action)
        {
            if (!player.HasTouhouPetsBuff())
                return;

            // 玩家侧分发：只对当前玩家启用的增强执行。
            foreach (EnhancementId enhanceId in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
            {
                if (EnhanceRegistry.TryGetEnhancement(enhanceId, out var enhancement))
                    action(enhancement);
            }
        }
        private static void ProcessDemonismAction(Action<BaseEnhance> action)
        {
            // 全局分发：对所有已注册增强执行（与具体玩家无关的 Always 钩子使用）。
            foreach (BaseEnhance enhance in EnhanceRegistry.AllEnhancements)
            {
                action(enhance);
            }
        }
        private static bool? ProcessDemonismAction(Player player, bool? priority, Func<BaseEnhance, bool?> action)
        {
            if (!player.HasTouhouPetsBuff())
                return null;

            if (priority == null)
            {
                bool? @return = null;
                foreach (EnhancementId enhanceId in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
                {
                    if (!EnhanceRegistry.TryGetEnhancement(enhanceId, out var enhancement))
                        continue;

                    bool? a = action(enhancement);
                    if (a != null) @return = a;
                }
                return @return;
            }
            else
            {
                bool? @return = null;
                foreach (EnhancementId enhanceId in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
                {
                    if (!EnhanceRegistry.TryGetEnhancement(enhanceId, out var enhancement))
                        continue;

                    bool? a = action(enhancement);
                    if (a == priority) return a;
                    else if (a != null) @return = a;
                }
                return @return;
            }
        }
        private static float ProcessDemonismAction(Player player, Func<BaseEnhance, float?> action)
        {
            float multiplier = 1f;

            if (!player.HasTouhouPetsBuff())
                return multiplier;

            foreach (EnhancementId enhanceId in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
            {
                if (!EnhanceRegistry.TryGetEnhancement(enhanceId, out var enhancement))
                    continue;

                multiplier *= action(enhancement) ?? 1f;
            }

            return multiplier;
        }
        public override void Initialize()
        {
            // Initialize：让增强有机会初始化玩家侧状态。
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerInitialize(Player));
        }
        public override void ResetEffects()
        {
            if (Player == Main.LocalPlayer && !ModContent.GetInstance<WitchTrial>().IsCloneable)
                ModContent.GetInstance<WitchTrial>().Condition.Value = 0;

            ActiveEnhanceCount = 1;

            if (NewlyMadeDoll)
                ActiveEnhanceCount++;

            if (ABurntDoll)
                ActiveEnhanceCount++;

            ProcessDemonismAction(Player, (enhance) => enhance.PlayerResetEffects(Player));
            ProcessDemonismAction((enhance) => enhance.PlayerResetEffectsAlways(Player));

            if (Player == Main.LocalPlayer)
            {
                var bigSevenStars = ModContent.GetInstance<BigSevenStars>();

                bigSevenStars.Condition.Value = ActiveEnhanceCount;
                if (bigSevenStars.Condition.Value >= BigSevenStars.Max)
                    bigSevenStars.Condition.Complete();
            }

            // 每 tick 重建被动启用列表，确保状态与背包/宠物栏一致。
            ActivePassiveEnhance = [];

            FragrantAromaFillsTheAir = false;

            if (Config.PetInv)
            {
                foreach (Item item in Player.miscEquips)
                {
                    Player.AddActivePassiveEnhance(item);
                }
            }

            ProcessItems(Player, Player.bank.item);
            ProcessItems(Player, Player.bank2.item);
            ProcessItems(Player, Player.bank3.item);

            static void ProcessItems(Player player, Item[] items)
            {
                Span<Item> span = items;
                for (int i = 0; i < span.Length; i++)
                {
                    ref Item item = ref span[i];
                    player.AddActivePassiveEnhance(item);
                }
            }

            ProcessDemonismAction((enhance) => enhance.PlayerPostResetEffects(Player));
        }
        public override void SaveData(TagCompound tag)
        {
            // 存档：把 EnhancementId 序列化为 string 列表（Value 是载体）。
            List<string> activeEnhanceIds = [];
            for (int i = 0; i < ActiveEnhance.Count; i++)
                activeEnhanceIds.Add(ActiveEnhance[i].Value);

            tag.Add("ActiveEnhanceIds", activeEnhanceIds);
            tag.Add("EatBook", EatBook);
            tag.Add("adjTileVanilla", adjTileVanilla);
            tag.Add("adjTileMod", adjTileMod);
            tag.Add("adjOther", adjOther);
            tag.Add("ExtraAddition", ExtraAddition);
            tag.Add("NewlyMadeDoll", NewlyMadeDoll);
            tag.Add("ABurntDoll", ABurntDoll);
        }
        public override void LoadData(TagCompound tag)
        {
            // 读档：优先读取新字段 ActiveEnhanceIds；若不存在则兼容旧字段 ActiveEnhanceName。
            List<EnhancementId> ids = [];
            if (tag.TryGet<List<string>>("ActiveEnhanceIds", out var loadedIds) && loadedIds != null)
            {
                for (int i = 0; i < loadedIds.Count; i++)
                {
                    string raw = loadedIds[i];
                    if (!string.IsNullOrWhiteSpace(raw))
                        ids.Add(EnhancementId.From(raw));
                }
            }
            else if (tag.TryGet<List<string>>("ActiveEnhanceName", out var legacyNames) && legacyNames != null)
            {
                foreach (string name in legacyNames)
                {
                    if (!ModContent.TryFind("TouhouPets", name, out ModItem item))
                        continue;
                    if (!EnhanceRegistry.TryGetEnhanceId(item.Type, out EnhancementId enhanceId))
                        continue;
                    ids.Add(enhanceId);
                }
            }

            ActiveEnhance = ids;
            EatBook = tag.GetInt("EatBook");
            NewlyMadeDoll = tag.GetBool("NewlyMadeDoll");
            ABurntDoll = tag.GetBool("ABurntDoll");
            if (tag.TryGet<bool[]>("adjTileVanilla", out var adjtileVanilla))
                adjTileVanilla = adjtileVanilla;
            else
                adjTileVanilla = new bool[TileID.Count];
            if (tag.TryGet<List<string>>("adjTileMod", out var adjtileMod))
                adjTileMod = adjtileMod;
            else
                adjTileMod = [];
            if (tag.TryGet<bool[]>("adjOther", out var adjother))
                adjOther = adjother;
            else
            adjOther = new bool[5];
            if (tag.GetIntArray("ExtraAddition").Length != 0) ExtraAddition = tag.GetIntArray("ExtraAddition");
            if (ExtraAddition.Length < ExtraAdditionMax.Length) Array.Resize(ref ExtraAddition, ExtraAdditionMax.Length);

            adjTile = (bool[])Player.adjTile.Clone();

            for (int i = 0; i < adjTileVanilla.Length; i++)
            {
                adjTile[i] = adjTileVanilla[i];
            }

            foreach (string fullName in adjTileMod)
            {
                string[] parts = fullName.Split('/');
                if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]) && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModTile tile))
                    adjTile[tile.Type] = true;
            }
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            return [new Item(ModContent.ItemType<MysteriousSlip>())];
        }
        public override void ModifyLuck(ref float luck)
        {
            ModifyLuck_luck = luck;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyLuck(Player, ref ModifyLuck_luck));
            luck = ModifyLuck_luck;
        }
        public override void UpdateLifeRegen()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerUpdateLifeRegen(Player));
        }
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            GetHealLife_healValue = healValue;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerGetHealLife(Player, item, quickHeal, ref GetHealLife_healValue));
            healValue = GetHealLife_healValue;
        }
        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            GetHealMana_healValue = healValue;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerGetHealMana(Player, item, quickHeal, ref GetHealMana_healValue));
            healValue = GetHealMana_healValue;
        }
        public override void PreUpdate()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPreUpdate(Player));
        }
        public override void PreUpdateBuffs()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPreUpdateBuffs(Player));
            ProcessDemonismAction((enhance) => enhance.PlayerPreUpdateBuffsAlways(Player));
        }
        public override void PostUpdateBuffs()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdateBuffs(Player));
        }
        public override void PostUpdateEquips()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdateEquips(Player));
        }
        public override void PostUpdateRunSpeeds()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdateRunSpeeds(Player));
        }
        public override void PostUpdate()
        {
            if (FragrantAromaFillsTheAir && TouhouPetsExModSystem.SynchronousTime % 60 == 37)
                Player.statLife += Math.Clamp(Player.statLifeMax2 / 100, 0, Player.statLifeMax2 - Player.statLife);

            if (Main.GameUpdateCount % 18000 == 12000 && Player == Main.LocalPlayer)
                AwardPlayerSync(Mod, -1, Main.myPlayer);

            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdate(Player));
        }
        public override void ModifyItemScale(Item item, ref float scale)
        {
            ModifyItemScale_scale = scale;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyItemScale(Player, item, ref ModifyItemScale_scale));
            scale = ModifyItemScale_scale;
        }
        public override float UseTimeMultiplier(Item item)
        {
            return ProcessDemonismAction(Player, (enhance) => enhance.PlayerUseTimeMultiplier(Player, item));
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            DrawEffects_r = r;
            DrawEffects_g = g;
            DrawEffects_b = b;
            DrawEffects_a = a;
            DrawEffects_fullBright = fullBright;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerDrawEffects(drawInfo, ref DrawEffects_r, ref DrawEffects_g, ref DrawEffects_b, ref DrawEffects_a, ref DrawEffects_fullBright));
            r = DrawEffects_r;
            g = DrawEffects_g;
            b = DrawEffects_b;
            a = DrawEffects_a;
            fullBright = DrawEffects_fullBright;
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            bool? reesult = ProcessDemonismAction(Player, true, (enhance) => enhance.PlayerFreeDodge(Player, info));

            return reesult ?? base.FreeDodge(info);
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (modifiers.DamageSource.SourceNPCIndex > -1)
            {
                NPC npc = Main.npc[modifiers.DamageSource.SourceNPCIndex];
                if (npc.GetGlobalNPC<GEnhanceNPCs>().MoonMist && Main.rand.NextBool(10))
                {
                    modifiers.Cancel();

                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TownSlimeTransform, new ParticleOrchestraSettings
                    {
                        UniqueInfoPiece = 1,
                        PositionInWorld = Player.Center + (npc.Center - Player.Center) / 4f,
                        MovementVector = Vector2.Zero
                    });

                    Player.immune = true;
                    Player.immuneTime += Player.longInvince ? 60 : 20;
                    for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                    {
                        Player.hurtCooldowns[i] += Player.longInvince ? 60 : 20;
                    }
                }
            }

            ModifyHurt_modifiers = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHurt(Player, ref ModifyHurt_modifiers));
            modifiers = ModifyHurt_modifiers;
        }
        public override void PostHurt(Player.HurtInfo info)
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostHurt(Player, info));
        }
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyHitNPCWithItem_modifiers = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHitNPCWithItem(Player, item, target, ref ModifyHitNPCWithItem_modifiers));
            modifiers = ModifyHitNPCWithItem_modifiers;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyHitNPCWithProj_modifiers = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHitNPCWithProjectile(Player, proj, target, ref ModifyHitNPCWithProj_modifiers));
            modifiers = ModifyHitNPCWithProj_modifiers;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyHitNPC_modifiers = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHitNPC(Player, target, ref ModifyHitNPC_modifiers));
            modifiers = ModifyHitNPC_modifiers;
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            PreKill_playSound = playSound;
            PreKill_genDust = genDust;
            PreKill_damageSource = damageSource;
            bool? result = ProcessDemonismAction(Player, false, (enhance) => enhance.PlayerPreKill(Player, damage, hitDirection, pvp, ref PreKill_playSound, ref PreKill_genDust, ref PreKill_damageSource));
            playSound = PreKill_playSound;
            genDust = PreKill_genDust;
            damageSource = PreKill_damageSource;

            return result ?? base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Player == Main.LocalPlayer)
            {
                ModContent.GetInstance<TouhouFairyKnockout>().Condition.Value = 0;
                ModContent.GetInstance<FacingTheMiracle>().Condition.Value = 0;
            }

            ProcessDemonismAction(Player, (enhance) => enhance.PlayerKill(Player, damage, hitDirection, pvp, damageSource));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerOnHitNPC(Player, target, hit, damageDone));
        }
        public override void OnEnterWorld()
        {
            if (Player == Main.LocalPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                // 客户端进世界时请求一次同步，保证本地状态与服务器一致。
                AwardPlayerSync(Mod, -1, Main.myPlayer, true);
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TouhouPetsExModSystem.ReisenKeyBind.JustPressed && Config.Reisen && Main.LocalPlayer.EnableEnhance<ReisenGun>())
            {
                List<int> blackList = [];
                foreach (string fullName in Config.Letty_2_1 ?? [])
                {
                    string[] parts = fullName.Split('/');
                    if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]) && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModProjectile proj))
                        blackList.Add(proj.Type);
                }

                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != Main.LocalPlayer.whoAmI || proj.type == ProjectileID.ChlorophyteBullet || (proj.type >= ProjectileID.Count && blackList.Contains(proj.type)) || !proj.GetGlobalProjectile<GEnhanceProjectile>().Bullet)
                        continue;

                    int target = proj.FindTargetWithLineOfSight();

                    if (target == -1)
                        continue;

                    proj.velocity = proj.DirectionTo(Main.npc[target].Center).SafeNormalize(-Vector2.UnitY) * proj.velocity.Length();
                    proj.netUpdate = true;
                    proj.GetGlobalProjectile<GEnhanceProjectile>().Bullet = false;
                }

                Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<ReisenEffect>(), 0, 0, Main.LocalPlayer.whoAmI, ai1: 1);
            }

            if (TouhouPetsExModSystem.KoishiKeyBind.JustPressed && Config.Koishi && Main.LocalPlayer.EnableEnhance<KoishiTelephone>())
            {
                if (Main.LocalPlayer.HasBuff(ModContent.BuffType<PopularityExplosion>()))
                    Main.LocalPlayer.ClearBuff(ModContent.BuffType<PopularityExplosion>());
                else
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<PopularityExplosion>(), 900);
                    Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<PopularityExplosionEffect>(), 0, 0, Main.myPlayer);
                }
            }
        }

        public static void AwardPlayerSync(Mod mod, int toWho, int fromWho, bool rebate = false)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            Player plr = Main.player[fromWho];
            EnhancePlayers player = plr.MP();
            ModPacket packet = mod.GetPacket();

            packet.Write((byte)TouhouPetsEx.MessageType.StatIncreasePlayerSync);
            packet.Write((byte)plr.whoAmI);
            packet.Write(rebate);
            packet.Write(player.NewlyMadeDoll);
            packet.Write(player.ABurntDoll);
            packet.Write(player.EatBook);
            packet.Write(player.ActiveEnhance.Count);
            for (int i = 0; i < player.ActiveEnhance.Count; i++)
                packet.Write(player.ActiveEnhance[i].Value);
            packet.Write(player.ExtraAddition.Length);
            for (int i = 0; i < player.ExtraAddition.Length; i++)
                packet.Write(player.ExtraAddition[i]);
            packet.Send(toWho, fromWho);

            //Main.NewText("发");
        }
        public static void ReceivePlayerSync(BinaryReader reader, int whoAmI, bool award)
        {
            Player plr = Main.player[whoAmI];
            EnhancePlayers player = plr.MP();

            bool newlyMadeDoll = reader.ReadBoolean();
            bool aBurntDoll = reader.ReadBoolean();
            int eatBook = reader.ReadInt32();

            int activeEnhanceCount = reader.ReadInt32();
            List<EnhancementId> activeEnhance = [];
            for (int i = 0; i < activeEnhanceCount; i++)
            {
                string id = reader.ReadString();
                if (!string.IsNullOrWhiteSpace(id))
                    activeEnhance.Add(EnhancementId.From(id));
            }

            int extraAdditionLength = reader.ReadInt32();
            int[] extraAddition = new int[extraAdditionLength];
            for (int i = 0; i < extraAdditionLength; i++)
                extraAddition[i] = reader.ReadInt32();

            if (whoAmI != Main.myPlayer)
            {
                player.NewlyMadeDoll = newlyMadeDoll;
                player.ABurntDoll = aBurntDoll;
                player.EatBook = eatBook;
                player.ExtraAddition = extraAddition;
                player.ActiveEnhance = activeEnhance;
            }

            if (award)
                AwardPlayerSync(TouhouPetsEx.Instance, whoAmI, Main.myPlayer);

            //Main.NewText("收");
        }
        public static void YukariTp(Player player, Vector2 newPos)
        {
            player.RemoveAllGrapplingHooks();
            player.StopVanityActions();

            if (player.shimmering || player.shimmerWet)
            {
                player.shimmering = false;
                player.shimmerWet = false;
                player.wet = false;
                player.ClearBuff(353);
            }

            SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_warpl") with { MaxInstances = 114514 }, player.Center);
            for (int i = player.width * player.height / 5; i >= 0; i--)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.TeleportationPotion);
                dust.scale = Main.rand.Next(20, 70) * 0.01f;

                if (i < 10)
                    dust.scale += 0.25f;

                if (i < 5)
                    dust.scale += 0.25f;
            }

            PressurePlateHelper.UpdatePlayerPosition(player);
            player.position = newPos;
            player.fallStart = (int)player.position.Y / 16;
            PressurePlateHelper.UpdatePlayerPosition(player);
            player.ResetAdvancedShadows();
            for (int i = 0; i < 3; i++)
            {
                player.UpdateSocialShadow();
            }

            player.oldPosition = player.position + player.BlehOldPositionFixer;

            player.MP().YukariCD = 60;

            SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_warpl") with { MaxInstances = 114514 }, player == Main.LocalPlayer ? null : player.Center);
            for (int i = player.width * player.height / 5; i >= 0; i--)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.TeleportationPotion);
                dust.scale = Main.rand.Next(20, 70) * 0.01f;

                if (i < 10)
                    dust.scale += 0.25f;

                if (i < 5)
                    dust.scale += 0.25f;
            }
        }
    }
}
