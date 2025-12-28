using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Lily : BaseEnhance
    {
        public override string Text => GetText("Lily");
        public override string[] ExperimentalText => [GetText("Lily_1")];
        public override bool[] Experimental => [Config.Lily];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<LilyOneUp>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            if (player.MP().LilyCD > 0)
                player.MP().LilyCD--;

            if (player.MP().LilyCD == 1)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_extend"), player.Center);

            if (Config.Lily)
                player.statLifeMax2 += 80;
        }
        public override bool? PlayerPreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (player.MP().LilyCD == 0)
            {
                int time = player.longInvince ? 600 : 360;

                player.statLife = 1;
                foreach (int buffType in player.buffType)
                {
                    if (Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                        player.ClearBuff(buffType);
                }
                player.breath = player.breathMax;
                player.lifeRegenTime += 3600;
                player.immune = true;
                player.immuneTime += time;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += time;
                }
                player.MP().LilyCD = 14400;
                Projectile.NewProjectile(player.GetSource_Death(), player.Center, Vector2.Zero, ModContent.ProjectileType<LilyDodgeEffects>(), 0, 0, player.whoAmI);

                if (player.EnableAllYousei() && player == Main.LocalPlayer)
                {
                    var touhouFairyKnockout = ModContent.GetInstance<TouhouFairyKnockout>();

                    if (touhouFairyKnockout.Condition.Value == 0)
                        touhouFairyKnockout.Condition.Value = 2;
                    else if (touhouFairyKnockout.Condition.Value == 1)
                        touhouFairyKnockout.Condition.Complete();
                }
                return false;
            }

            return null;
        }
        public override void PlayerKill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            player.MP().LilyCD = 0;
        }
    }
}
