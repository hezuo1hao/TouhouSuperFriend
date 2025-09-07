using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;
using static TouhouPetsEx.TouhouPetsEx;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Sanae : BaseEnhance
    {
        public override string Text => GetText("Sanae");
        public override bool Passive => true;
        public override bool[] Experimental => [Config.Sanae];
        public override string[] ExperimentalText => [GetText("Sanae_1")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SanaeCoin>());
        }
        public override void PlayerKill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Config.Sanae)
                player.MP().SanaeCD = 0;
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (!Config.Sanae)
                return;

            if (player.MP().SanaeCD > 0 && player.statLife == player.statLifeMax2)
                player.MP().SanaeCD--;

            if (player.MP().SanaeCD == 1)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/Power"), player.Center);

            if (player.statLife < player.statLifeMax2 / 2 && player.MP().SanaeCD == 0)
            {
                player.MP().SanaeCD = 21600;
                player.Heal(player.statLifeMax2 / 2);
                foreach (int buffType in player.buffType)
                {
                    if (Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                        player.ClearBuff(buffType);
                }
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<SanaeRegen>(), 0, 0, player.whoAmI);
            }
        }
        public override bool? ItemCanUseItem(Item item, Player player, ref bool def)
        {
            def = false;

            if (player.altFunctionUse == 2 && item.type == ModContent.ItemType<SanaeCoin>())
            {
                if (Main.MouseWorld.Y < Main.screenPosition.Y + Main.screenHeight / 3f)
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
