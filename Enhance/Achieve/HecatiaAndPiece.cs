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

            int magnification = EnhanceCount[ModContent.ItemType<HecatiaPlanet>()];

            if (magnification == 0)
                return;

            // 总体设置
            int cycle = 60;  // 总的时间为 60 帧
            int damage = 0;
            int Radius = 15;
            int minTileX = (int)(npc.Center.X / 16f - Radius);
            int maxTileX = (int)(npc.Center.X / 16f + Radius);
            int minTileY = (int)(npc.Center.Y / 16f - Radius);
            int maxTileY = (int)(npc.Center.Y / 16f + Radius);

            // 确保坐标在有效范围内
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

            // 计算应该在这一帧执行的任务数量
            int tilesPerFrame = ((maxTileX - minTileX + 1) * (maxTileY - minTileY + 1)) / cycle;
            int currentStep = (int)(Main.GameUpdateCount % cycle);

            // 逐步执行循环
            int startTileIndex = currentStep * tilesPerFrame;
            int endTileIndex = (currentStep + 1) * tilesPerFrame - 1;

            // 遍历指定区域的所有瓷砖
            int tileIndex = 0;
            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    if (tileIndex >= startTileIndex && tileIndex <= endTileIndex)
                    {
                        // 计算到 NPC 的距离
                        float diffX = Math.Abs(i - npc.position.X / 16f);
                        float diffY = Math.Abs(j - npc.position.Y / 16f);
                        double distanceToTile = Math.Sqrt(diffX * diffX + diffY * diffY);

                        // 判断是否在范围内且是火把类型的砖块
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
