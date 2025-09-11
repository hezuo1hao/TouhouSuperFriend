using Humanizer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.WorldBuilding;
using TouhouPets.Content.Items.PetItems;
using TouhouPets.Content.Projectiles.Pets;
using TouhouPetsEx.Projectiles;
using static Terraria.Localization.NetworkText;

namespace TouhouPetsEx.Enhance.Core
{
	public class EnhanceOn : ModSystem
    {
        public override void Load()
        {
            On_Player.GetDamage += On_Player_GetDamage;
            On_Player.GetCritChance += On_Player_GetCritChance;
            On_Player.GetAttackSpeed += On_Player_GetAttackSpeed;
            On_Player.GetKnockback += On_Player_GetKnockback;
            On_Player.GetArmorPenetration += On_Player_GetArmorPenetration;
            On_Player.VanillaBaseDefenseEffectiveness += On_Player_VanillaBaseDefenseEffectiveness;
            On_Player.GetPickaxeDamage += On_Player_GetPickaxeDamage;
            On_Player.RollLuck += LuckUp;
            On_NPC.HitModifiers.GetDamage += ExCrit;
            On_NPC.getGoodAdjustments += On_NPC_getGoodAdjustments;
            On_NPC.StrikeNPC_HitInfo_bool_bool += SuperCrit;
            On_NPC.NPCLoot_DropMoney += LuckUp;
            Main.OnPostFullscreenMapDraw += TeleportFromMap;
            On_Main.DamageVar_float_int_float += LuckUp;
            On_WorldGen.ShakeTree += On_WorldGen_ShakeTree;
            On_Player.AdjTiles += On_Player_AdjTiles;
            On_BirthdayParty.NaturalAttempt += On_BirthdayParty_NaturalAttempt;
            On_Main.UpdateTime_StartNight += On_Main_UpdateTime_StartNight;
            On_WorldGen.UpdateWorld_GrassGrowth += On_WorldGen_UpdateWorld_GrassGrowth;
            On_ShopHelper.LimitAndRoundMultiplier += On_ShopHelper_LimitAndRoundMultiplier;
            On_ShopHelper.ProcessMood += On_ShopHelper_ProcessMood;

            // 锤子敲背景墙有特殊处理，要用On才能应用工具速度提升
            On_Player.ItemCheck_UseMiningTools_TryHittingWall += (orig, player, item, x, y) =>
            {
                orig.Invoke(player, item, x, y);

                // 检测那一堆if判断成没成
                if (player.itemTime == item.useTime / 2 && player.EnableEnhance<FlandrePudding>())
                    player.itemTime = (int)Math.Max(1, item.useTime / 4f);
            };
        }

        private void On_ShopHelper_ProcessMood(On_ShopHelper.orig_ProcessMood orig, ShopHelper self, Player player, NPC npc)
        {
            if (player.EnableEnhance<KokoroMask>())
            {
                bool a = Main.remixWorld;
                Main.remixWorld = false;
                orig(self, player, npc);
                Main.remixWorld = a;
            }
            else orig(self, player, npc);
        }

        private float On_ShopHelper_LimitAndRoundMultiplier(On_ShopHelper.orig_LimitAndRoundMultiplier orig, ShopHelper self, float priceAdjustment)
        {
            if (Main.LocalPlayer.EnableEnhance<KokoroMask>())
                return 0.75f;

            return orig(self, priceAdjustment);
        }

        private void On_WorldGen_UpdateWorld_GrassGrowth(On_WorldGen.orig_UpdateWorld_GrassGrowth orig, int i, int j, int minI, int maxI, int minJ, int maxJ, bool underground)
        {
            orig(i, j, minI, maxI, minJ, maxJ, underground);

            if (!underground || !WorldGen.InWorld(i, j, 10) || Main.tile[i, j].type != TileID.JungleGrass)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.sleeping.isSleeping || !player.EnableEnhance<DoremyPillow>())
                    return;

                orig(i, j, minI, maxI, minJ, maxJ, underground);
            }
        }

        private void On_Main_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
        {
            orig(ref stopEvents);

            if (Main.netMode == NetmodeID.MultiplayerClient || !NPC.downedBoss2)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.sleeping.isSleeping && player.EnableEnhance<DoremyPillow>() && Main.rand.NextBool(50))
                    WorldGen.spawnMeteor = true;
            }
        }

        private void On_BirthdayParty_NaturalAttempt(On_BirthdayParty.orig_NaturalAttempt orig)
        {
            orig();

            if (Main.netMode == NetmodeID.MultiplayerClient || !NPC.AnyNPCs(208) || BirthdayParty.PartyDaysOnCooldown > 0)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.sleeping.isSleeping && player.EnableEnhance<DoremyPillow>())
                    orig();
            }
        }

        private void On_Player_AdjTiles(On_Player.orig_AdjTiles orig, Player self)
        {
            orig(self);

            if (!self.EnableEnhance<NitoriCucumber>())
                return;

            EnhancePlayers mp = self.MP();

            for (int i = 0; i < self.adjTile.Length; i++)
            {
                if (!self.adjTile[i])
                    continue;

                mp.adjTile[i] = true;

                if (i < TileID.Count)
                    mp.adjTileVanilla[i] = true;
                else if (!mp.adjTileMod.Contains(TileLoader.GetTile(i).FullName))
                    mp.adjTileMod.Add(TileLoader.GetTile(i).FullName);
            }

            if (self.adjWater)
                mp.adjOther[0] = true;

            if (self.adjHoney)
                mp.adjOther[1] = true;

            if (self.adjLava)
                mp.adjOther[2] = true;

            if (self.adjShimmer)
                mp.adjOther[3] = true;

            if (self.alchemyTable)
                mp.adjOther[4] = true;

            self.adjWater = mp.adjOther[0];
            self.adjHoney = mp.adjOther[1];
            self.adjLava = mp.adjOther[2];
            self.adjShimmer = mp.adjOther[3];
            self.alchemyTable = mp.adjOther[4];
            self.adjTile = (bool[])mp.adjTile.Clone();
            Recipe.FindRecipes();
        }

        private void On_WorldGen_ShakeTree(On_WorldGen.orig_ShakeTree orig, int i, int j)
        {
            if (!WorldEnableEnhance<SizuhaBrush>())
            {
                orig(i, j);
                return;
            }

            for (int l = 0; ;l++)
            {
                // 在orig前获取树是否被摇过，因为orig会修改WorldGen.treeShakeX,Y的值，标记为被摇过
                bool treeShaken = false;

                WorldGen.GetTreeBottom(i, j, out var x, out var y);
                for (int k = 0; k < WorldGen.numTreeShakes; k++)
                {
                    if (WorldGen.treeShakeX[k] == x && WorldGen.treeShakeY[k] == y)
                    {
                        treeShaken = true;
                        break;
                    }
                }

                bool a = Main.getGoodWorld;
                Main.getGoodWorld = false;

                orig(i, j);

                Main.getGoodWorld = a;

                if (WorldGen.numTreeShakes == WorldGen.maxTreeShakes || treeShaken || l >= 20)
                    return;
                else
                {
                    for (int k = 0; k < WorldGen.numTreeShakes; k++)
                    {
                        if (WorldGen.treeShakeX[k] == x && WorldGen.treeShakeY[k] == y)
                        {
                            if (k < WorldGen.numTreeShakes - 1)
                            {
                                Array.Copy(WorldGen.treeShakeX, k + 1, WorldGen.treeShakeX, k, WorldGen.numTreeShakes - k - 1);
                                Array.Copy(WorldGen.treeShakeY, k + 1, WorldGen.treeShakeY, k, WorldGen.numTreeShakes - k - 1);
                            }

                            WorldGen.treeShakeX[WorldGen.numTreeShakes - 1] = 0;
                            WorldGen.treeShakeY[WorldGen.numTreeShakes - 1] = 0;

                            WorldGen.numTreeShakes--;
                            break;
                        }
                    }
                }
            }
        }

        private int LuckUp(On_Main.orig_DamageVar_float_int_float orig, float dmg, int percent, float luck)
        {
            int damage = orig(dmg, percent, luck);

            if (luck > 1f && Main.LocalPlayer.EnableEnhance<TewiCarrot>() && Main.rand.NextFloat() < luck - 1)
            {
                float damage2 = dmg * (1f + Main.rand.Next(-percent, percent * 2 + 1) * 0.01f);
                if (damage2 > damage)
                    damage = (int)Math.Round(damage2);
            }

            if (luck < -1f && Main.LocalPlayer.EnableEnhance<TewiCarrot>() && Main.rand.NextFloat() < -1 - luck)
            {
                float damage2 = dmg * (1f + Main.rand.Next(-percent * 2, percent + 1) * 0.01f);
                if (damage2 < damage)
                    damage = (int)Math.Round(damage2);
            }

            if (luck > 0f && damage < dmg && Main.LocalPlayer.EnableEnhance<HinaDoll>())
                damage = (int)Math.Round(dmg * (1f + Main.rand.Next(percent + 1) * 0.01f));

            if (luck < 0f && damage > dmg && Main.LocalPlayer.EnableEnhance<HinaDoll>())
                damage = (int)Math.Round(dmg * (1f + Main.rand.Next(-percent, 1) * 0.01f));

            return damage;
        }

        private void LuckUp(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
        {
            float value = self.value;

            if (closestPlayer.luck > 1f && closestPlayer.EnableEnhance<TewiCarrot>())
                self.value *= 1 + MathHelper.Lerp(0, 0.05f + Main.rand.NextFloat(0.20f), closestPlayer.luck - 1);

            orig(self, closestPlayer);

            self.value = value;
        }

        private int LuckUp(On_Player.orig_RollLuck orig, Player self, int range)
        {
            if (self.luck > 1f && self.EnableEnhance<TewiCarrot>() && Main.rand.NextFloat() < self.luck - 1)
                return Main.rand.Next(Main.rand.Next(range / 4, range));

            return orig(self, range);
        }

        private int SuperCrit(On_NPC.orig_StrikeNPC_HitInfo_bool_bool orig, NPC self, NPC.HitInfo hit, bool fromNet, bool noPlayerInteraction)
        {
            if (self.GetGlobalNPC<GEnhanceNPCs>().SuperCrit)
            {
                int index = NewText(new Rectangle((int)self.position.X, (int)self.position.Y, self.width, self.height), new Color(133, 0, 133), hit.Damage, true);

                if (index <= Main.combatText.Length - 1)
                {
                    var text = Main.combatText[index];
                    text.lifeTime *= 2;
                    text.rotation *= 2;
                    hit.HideCombatText = true;
                }
                self.GetGlobalNPC<GEnhanceNPCs>().SuperCrit = false;
            }

            return orig(self, hit, fromNet, noPlayerInteraction);
        }

        private void On_NPC_getGoodAdjustments(On_NPC.orig_getGoodAdjustments orig, NPC self)
        {
            orig(self);

            if (!WorldEnableEnhance<SuikaGourd>())
                return;

            bool a = Main.tenthAnniversaryWorld;
            NPC npc = new();
            Main.getGoodWorld = false;
            Main.tenthAnniversaryWorld = false;
            npc.SetDefaults(self.type);
            self.scale = npc.scale;
            Main.getGoodWorld = true;
            Main.tenthAnniversaryWorld = a;
        }

        private void TeleportFromMap(Vector2 arg1, float arg2)
        {
            if (!Main.mouseRight || (Main.CurrentFrameFlags.AnyActiveBossNPC && !Config.Yukari) || Main.LocalPlayer.MP()?.YukariCD is null or > 0 || !Main.LocalPlayer.EnableEnhance<YukarisItem>())
                return;

            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;
            Vector2 target = ((Main.MouseScreen - screenSize / 2) / 16 * (16 / Main.mapFullscreenScale) + Main.mapFullscreenPos) * 16;

            if (WorldGen.InWorld((int)target.X / 16, (int)target.Y / 16) && Main.LocalPlayer.BuyItem(10000))
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    EnhancePlayers.YukariTp(Main.LocalPlayer, target);
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    EnhancePlayers.YukariTp(Main.LocalPlayer, target);

                    ModPacket packet = Mod.GetPacket();

                    packet.Write((byte)TouhouPetsEx.MessageType.Tp);
                    packet.Write((byte)Main.LocalPlayer.whoAmI);
                    packet.WriteVector2(target);
                    packet.Send(-1, Main.LocalPlayer.whoAmI);
                }
            }
        }

        private int ExCrit(On_NPC.HitModifiers.orig_GetDamage orig, ref NPC.HitModifiers self, float baseDamage, bool crit, bool damageVariation, float luck)
        {
            if (!crit && self.DamageType?.CountsAsClass(DamageClass.Summon) == true && Main.LocalPlayer.EnableEnhance<YukarisItem>() && Main.rand.NextBool(100))
            {
                self.SetCrit();
            }

            if (Main.LocalPlayer.EnableEnhance<TenshiKeyStone>())
            {
                self.ScalingArmorPenetration += baseDamage / 100f;

                if (Config.Tenshi)
                {
                    float defense = self.Defense.ApplyTo(0);
                    float effectiveScaling = Math.Max(self.ScalingArmorPenetration.Value, 0);
                    float totalPenetration = effectiveScaling * defense + self.ArmorPenetration.Value;

                    if (effectiveScaling > 1)
                        totalPenetration = effectiveScaling * self.ArmorPenetration.Value;

                    baseDamage += Math.Max(totalPenetration - defense, 0);
                }
            }
            return orig(ref self, baseDamage, crit, damageVariation, luck);
        }
        private int On_Player_GetPickaxeDamage(On_Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            int damage = orig(self, x, y, pickPower, hitBufferIndex, tileTarget);

            if (damage > 0 && self.EnableEnhance<FlandrePudding>())
            {
                damage *= 495;
            }

            return damage;
        }

        private float On_Player_VanillaBaseDefenseEffectiveness(On_Player.orig_VanillaBaseDefenseEffectiveness orig)
        {
            float effectiveness = orig();

            if (Main.LocalPlayer.MP()?.FragrantAromaFillsTheAir == true)
                effectiveness += 0.25f;

            return effectiveness;
        }

        private ref float On_Player_GetArmorPenetration(On_Player.orig_GetArmorPenetration orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].armorPen;

            return ref orig(self, damageClass);
        }

        private ref StatModifier On_Player_GetKnockback(On_Player.orig_GetKnockback orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].knockback;

            return ref orig(self, damageClass);
        }

        private ref float On_Player_GetAttackSpeed(On_Player.orig_GetAttackSpeed orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].attackSpeed;

            return ref orig(self, damageClass);
        }

        private ref float On_Player_GetCritChance(On_Player.orig_GetCritChance orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].critChance;

            return ref orig(self, damageClass);
        }

        private ref StatModifier On_Player_GetDamage(On_Player.orig_GetDamage orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].damage;

            return ref orig(self, damageClass);
        }
    }
}
