using System;
using System.Collections.Generic;
using System.Linq;
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
        public override bool[] Experimental  => [Config.Hecatia];
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
            if (Main.netMode == NetmodeID.MultiplayerClient || npc.dontTakeDamage || npc.friendly || NPCID.Sets.CountsAsCritter[npc.type])
                return;

            if (EnhanceCount == null)
                return;

            if (!EnhanceRegistry.TryGetEnhanceId(ModContent.ItemType<HecatiaPlanet>(), out EnhancementId enhanceId))
                return;

            EnhanceCount.TryGetValue(enhanceId, out int magnification);

            if (magnification == 0)
                return;

            // 火把伤害：分摊扫描，避免每帧全量遍历
            int cycle = 60;  // 总周期：60 帧
            int damage = 0;
            int Radius = 225; // 扫描半径（单位：tile）
            int minTileX = (int)(npc.Center.X / 16f - Radius);
            int maxTileX = (int)(npc.Center.X / 16f + Radius);
            int minTileY = (int)(npc.Center.Y / 16f - Radius);
            int maxTileY = (int)(npc.Center.Y / 16f + Radius);

            // 确保范围在世界边界内
            if (minTileX < 0)
            {
                minTileX = 0;
            }
            if (maxTileX > Main.maxTilesX)
            {
                maxTileX = Main.maxTilesX;
            }
            if (minTileY < 0)
            {
                minTileY = 0;
            }
            if (maxTileY > Main.maxTilesY)
            {
                maxTileY = Main.maxTilesY;
            }

            // 将扫描分摊到每一帧
            int tilesPerFrame = ((maxTileX - minTileX + 1) * (maxTileY - minTileY + 1)) / cycle;
            int currentStep = (int)(Main.GameUpdateCount % cycle);

            // 本帧扫描的索引区间
            int startTileIndex = currentStep * tilesPerFrame;
            int endTileIndex = (currentStep + 1) * tilesPerFrame - 1;

            // 遍历指定范围内的 tile（仅处理本帧分配到的部分）
            int tileIndex = 0;
            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    if (tileIndex >= startTileIndex && tileIndex <= endTileIndex)
                    {
                        // 计算 tile 到 NPC 的距离平方（以 tile 为单位）
                        float diffX = Math.Abs(i - npc.position.X / 16f);
                        float diffY = Math.Abs(j - npc.position.Y / 16f);
                        double distanceToTile = diffX * diffX + diffY * diffY;

                        // 范围内且为火把 tile 则计数
                        if (distanceToTile < Radius && TileID.Sets.Torch[Framing.GetTileSafely(i, j).TileType])
                        {
                            damage++;
                        }
                    }
                    tileIndex++;
                }
            }

            if (damage != 0)
                npc.GetGlobalNPC<GEnhanceNPCs>().TorchDamage += (int)Math.Ceiling(damage / 10f) * magnification;

            if (Main.GameUpdateCount % 30 == 17 && npc.GetGlobalNPC<GEnhanceNPCs>().TorchDamage > 0)
            {
                npc.SimpleStrikeNPC(npc.GetGlobalNPC<GEnhanceNPCs>().TorchDamage, 0);
                npc.GetGlobalNPC<GEnhanceNPCs>().TorchDamage = 0;
            }
        }
    }
}
