using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;
using static TouhouPetsEx.TouhouPetsEx;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Sanae : BaseEnhance
    {
        public override string Text => GetText("Sanae");
        public override bool Passive => true;
        public override bool[] Experimental => [Config.Sanae, Config.Sanae_2];
        public override string[] ExperimentalText => [GetText("Sanae_1"), GetText("Sanae_2")];
        public override bool EnableBuffText => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SanaeCoin>());
        }
        public override void PlayerPreUpdateBuffsAlways(Player player)
        {
            if (!player.HasBuff<SanaeCD>())
                return;

            if (player.buffTime[player.FindBuffIndex(ModContent.BuffType<SanaeCD>())] <= 5)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/Power") with { SoundLimitBehavior = SoundLimitBehavior.IgnoreNew}, player.Center);
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (!Config.Sanae)
                return;

            if (!player.dead && player.statLife < player.statLifeMax2 / 2 && !player.HasBuff<SanaeCD>())
            {
                player.Heal(player.statLifeMax2 / 2);
                foreach (int buffType in player.buffType)
                {
                    if (Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                        player.ClearBuff(buffType);
                }
                player.AddBuff(ModContent.BuffType<SanaeCD>(), 21600);
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<SanaeRegen>(), 0, 0, player.whoAmI);

                if (player == Main.LocalPlayer)
                {
                    var facingTheMiracle = ModContent.GetInstance<FacingTheMiracle>();
                    facingTheMiracle.Condition.Value++;

                    if (facingTheMiracle.Condition.Value == FacingTheMiracle.Max)
                        facingTheMiracle.Condition.Complete();
                }
            }
        }
        public override void PlayerPreUpdate(Player player)
        {
            if (!Config.Sanae_2 || Main.dayTime || player.RollGoodLuck(250) > 0)
                return;

            Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter.X + Main.rand.Next(-1000, 1000), Math.Max(player.MountedCenter.Y - 1750, 100), Main.rand.NextFloat(-5.000f, 5.000f), 15, ProjectileID.FallingStar, 1000, 0, player.whoAmI);
        }
        public override void PlayerPostUpdateAlways(Player player)
        {
            if (player.statLife < player.statLifeMax2 || !player.HasBuff<SanaeCD>())
                return;

            int buffIndex = player.FindBuffIndex(ModContent.BuffType<SanaeCD>());
            if (player.townNPCs > 2 && !NPC.AnyDanger())
                player.buffTime[buffIndex] -= 3;

            player.Kaguya(buffIndex);
        }
        public override bool? ItemCanUseItem(Item item, Player player, ref bool def)
        {
            if (player.altFunctionUse == 2 && item.type == ModContent.ItemType<SanaeCoin>())
            {
                def = false;

                if (Main.MouseWorld.Y < Main.screenPosition.Y + Main.screenHeight / 2f)
                {
                    if (!Main.raining)
                    {
                        Main.StartRain();
                        CombatText.NewText(player.getRect(), new Color(0, 255, 0), GetText("Sanae_0_1"));
                    }
                    else
                    {
                        Main.StopRain();
                        CombatText.NewText(player.getRect(), new Color(0, 255, 0), GetText("Sanae_0_2"));
                    }

                    Main.SyncRain();
                }
                else
                {
                    if (Main.MouseWorld.X > Main.screenPosition.X + Main.screenWidth / 5f * 2 && Main.MouseWorld.X < Main.screenPosition.X + Main.screenWidth / 5f * 3)
                    {
                        Main.windSpeedTarget = 0;
                        CombatText.NewText(player.getRect(), new Color(0, 255, 0), GetText("Sanae_0_4"));
                    }
                    else
                    {
                        Main.windSpeedTarget = (Main.MouseWorld.X - Main.screenPosition.X) / Main.screenWidth * 1.2f - 0.6f;
                        CombatText.NewText(player.getRect(), new Color(0, 255, 0), GetText("Sanae_0_3"));
                    }
                }

                if (Main.netMode == NetmodeID.SinglePlayer)
                    return false;

                ModPacket packet = Instance.GetPacket();

                packet.Write((byte)MessageType.Weather);
                packet.Write(Main.rainTime);
                packet.Write(Main.maxRaining);
                packet.Write(Main.raining);
                packet.Write(Main.windSpeedTarget);
                packet.Send(-1, player.whoAmI);
                return false;
            }

            return null;
        }
        public override void SystemPostUpdateEverything()
        {
            if (WorldEnableEnhance<SanaeCoin>())
            {
                Main.sundialCooldown = 0;
                Main.moondialCooldown = 0;
            }
        }
    }
}
