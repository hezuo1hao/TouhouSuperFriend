using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class HecatiaAndPiece : BaseEnhance
    {
        public override string Text => TouhouPetsExUtils.GetText("HecatiaAndPiece");
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<HecatiaPlanet>());
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<HecatiaPlanet>() && !player.MP().ActiveEnhance.Contains(ModContent.ItemType<HecatiaPlanet>()))
            {
                player.MP().ActiveEnhanceCount++;
                player.MP().ActiveEnhance.Add(ModContent.ItemType<HecatiaPlanet>());
            }
        }
        public override void PlayerPreUpdate(Player player)
        {
            if (Main.hardMode)
                player.MP().ActiveEnhanceCount++;

            if (NPC.downedMoonlord)
                player.MP().ActiveEnhanceCount++;
        }
        public override void NPCAI(NPC npc, Player player)
        {
            int cycle = 60; // 周期
            int Radius = 15;
            int minTileX = (int)(npc.position.X / 16f - Radius);
            int maxTileX = (int)(npc.position.X / 16f + Radius);
            int minTileY = (int)(npc.position.Y / 16f - Radius);
            int maxTileY = (int)(npc.position.Y / 16f + Radius);

            // 确保坐标范围在有效范围内
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

            // 当前周期中的步骤
            int totalTiles = (maxTileX - minTileX + 1) * (maxTileY - minTileY + 1);
            int stepCount = totalTiles / cycle;  // 每次更新时处理多少砖块

            if (stepCount == 0) stepCount = 1;  // 防止除以0，确保至少处理一个砖块

            int stepIndex = (int)(Main.time % cycle); // 当前时间所处的周期步骤

            // 计算当前步骤要处理的砖块
            int startTile = minTileX + (stepIndex * stepCount); // 从哪个砖块开始
            int endTile = startTile + stepCount - 1;  // 到哪个砖块结束

            // 限制结束位置不超出范围
            if (endTile > maxTileX) endTile = maxTileX;

            // 逐步执行每个砖块的操作
            for (int i = startTile; i <= endTile; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    float diffX = Math.Abs(i - npc.position.X / 16f);
                    float diffY = Math.Abs(j - npc.position.Y / 16f);
                    double distanceToTile = Math.Sqrt((double)(diffX * diffX + diffY * diffY));

                    // 如果距离小于设定的半径，并且该位置是火炬类型的砖块
                    if (distanceToTile < Radius && TileID.Sets.Torch[Framing.GetTileSafely(i, j).TileType])
                    {
                        npc.SimpleStrikeNPC(1, 0); // 对 NPC 造成伤害
                    }

                    // 生成火炬的粉尘效果
                    Dust.NewDustDirect(new Microsoft.Xna.Framework.Vector2(i * 16, j * 16), 1, 1, DustID.Torch).noGravity = true;
                }
            }
        }
    }
}
