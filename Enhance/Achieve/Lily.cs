using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Lily : BaseEnhance
    {
        public override string Text => GetText("Lily");
        public override string ExperimentalText => GetText("Lily_1");
        public override bool Experimental { get => Config.Lily; }
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

            if (Experimental)
                player.statLifeMax2 += 80;
        }
        public override bool? PlayerPreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (player.MP().LilyCD == 0)
            {
                player.statLife += 1;
                foreach (int buffType in player.buffType)
                {
                    if (Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                        player.ClearBuff(buffType);
                }
                player.lifeRegenTime += 900;
                player.immune = true;
                player.immuneTime += 360;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += 360;
                }
                player.MP().LilyCD = 14400;
                Projectile.NewProjectile(player.GetSource_Death(), player.Center, Vector2.Zero, ModContent.ProjectileType<LilyDodgeEffects>(), 0, 0, player.whoAmI);
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
