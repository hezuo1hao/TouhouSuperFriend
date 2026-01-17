using Microsoft.Xna.Framework;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Wriggle : BaseEnhance
    {
        public override string Text => GetText("Wriggle");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<WriggleInAJar>());
        }
        public override void PlayerResetEffects(Player player)
        {
            if (player.MP().WriggleCD > 0)
                player.MP().WriggleCD--;
        }
        public override void SystemPostUpdateNPCs()
        {
            NPC.goldCritterChance = 400;

            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.EnableEnhance<WriggleInAJar>())
                    continue;

                NPC.goldCritterChance /= 2;
                if (NPC.goldCritterChance <= 0)
                    NPC.goldCritterChance = 1;

                if (player.MP().WriggleCD > 0 || !Main.rand.NextBool(60))
                    continue;

                bool flag = false;
                int num10 = 0;
                int num11 = 0;
                int num12 = (int)(player.position.X / 16f) - NPC.spawnRangeX * 2;
                int num13 = (int)(player.position.X / 16f) + NPC.spawnRangeX * 2;
                int num14 = (int)(player.position.Y / 16f) - NPC.spawnRangeY * 2;
                int num15 = (int)(player.position.Y / 16f) + NPC.spawnRangeY * 2;
                int num16 = (int)(player.position.X / 16f) - NPC.safeRangeX;
                int num17 = (int)(player.position.X / 16f) + NPC.safeRangeX;
                int num18 = (int)(player.position.Y / 16f) - NPC.safeRangeY;
                int num19 = (int)(player.position.Y / 16f) + NPC.safeRangeY;
                if (num12 < 0)
                    num12 = 0;

                if (num13 > Main.maxTilesX)
                    num13 = Main.maxTilesX;

                if (num14 < 0)
                    num14 = 0;

                if (num15 > Main.maxTilesY)
                    num15 = Main.maxTilesY;

                for (int m = 0; m < 1000; m++)
                {
                    for (int n = 0; n < 100; n++)
                    {
                        int num20 = Main.rand.Next(num12, num13);
                        int num21 = Main.rand.Next(num14, num15);
                        if (!Main.tile[num20, num21].HasUnactuatedTile || !Main.tileSolid[Main.tile[num20, num21].TileType])
                        {
                            if ((Main.wallHouse[Main.tile[num20, num21].wall] && m < 999))
                                continue;

                            for (int num22 = num21; num22 < Main.maxTilesY; num22++)
                            {
                                if (Main.tile[num20, num22].HasUnactuatedTile && Main.tileSolid[Main.tile[num20, num22].TileType])
                                {
                                    if ((num20 < num16 || num20 > num17 || num22 < num18 || num22 > num19 || m == 999) && ((num20 >= num12 && num20 <= num13 && num22 >= num14 && num22 <= num15) || m == 999))
                                    {
                                        _ = Main.tile[num20, num22].type;
                                        num10 = num20;
                                        num11 = num22;
                                        flag = true;
                                    }

                                    break;
                                }
                            }

                            if (flag && m < 999)
                            {
                                int num24 = num10 - NPC.spawnSpaceX / 2;
                                int num25 = num10 + NPC.spawnSpaceX / 2;
                                int num26 = num11 - NPC.spawnSpaceY;
                                int num27 = num11;
                                if (num24 < 0)
                                    flag = false;

                                if (num25 > Main.maxTilesX)
                                    flag = false;

                                if (num26 < 0)
                                    flag = false;

                                if (num27 > Main.maxTilesY)
                                    flag = false;

                                if (flag)
                                {
                                    for (int num28 = num24; num28 < num25; num28++)
                                    {
                                        for (int num29 = num26; num29 < num27; num29++)
                                        {
                                            if (Main.tile[num28, num29].HasUnactuatedTile && Main.tileSolid[Main.tile[num28, num29].TileType])
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (flag)
                            break;
                    }

                    if (flag && m < 999)
                    {
                        Rectangle rectangle = new(num10 * 16, num11 * 16, 16, 16);
                        foreach (Player player1 in Main.ActivePlayers)
                        {
                            Rectangle rectangle2 = new((int)(player1.position.X + player1.width / 2 - NPC.sWidth / 2 - NPC.safeRangeX), (int)(player1.position.Y + player1.height / 2 - NPC.sHeight / 2 - NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                            if (rectangle.Intersects(rectangle2))
                                flag = false;
                        }
                    }

                    if (flag)
                        break;
                }

                if (flag)
                {
                    int spawnPositionX = num10 * 16 + 8;
                    int spawnPositionY = num11 * 16;
                    NPC.NewNPC(player.GetSource_FromThis(), spawnPositionX, spawnPositionY, Main.rand.Next(player.RollLuck(NPC.goldCritterChance) == 0 ? TouhouPetsEx.VanillaGoldBug : TouhouPetsEx.VanillaBug));
                    player.MP().WriggleCD = 300;
                }
            }
        }
    }
}
