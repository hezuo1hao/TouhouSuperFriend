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
    public class Daiyousei : BaseEnhance
    {
        public override string Text => GetText("Daiyousei");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<DaiyouseiBomb>());
        }
        public override void PlayerResetEffects(Player player)
        {
            if (player.MP().DaiyouseiCD > 0)
                player.MP().DaiyouseiCD--;

            if (player.MP().DaiyouseiCD == 1)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_cardget"), player.Center);
        }
        public override bool? PlayerFreeDodge(Player player, Player.HurtInfo info)
        {
            if (player.MP().DaiyouseiCD == 0)
            {
                player.immune = true;
                player.immuneTime += 180;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += 180;
                }
                player.MP().DaiyouseiCD = 5400;
                Projectile.NewProjectile(player.GetSource_OnHurt(info.DamageSource), player.Center, Vector2.Zero, ModContent.ProjectileType<DaiyouseiBoom>(), (info.Damage + 10) * 10, 5, player.whoAmI);

                if (player.EnableAllYousei())
                {
                    var touhouFairyKnockout = ModContent.GetInstance<TouhouFairyKnockout>();

                    if (touhouFairyKnockout.Condition.Value == 0)
                        touhouFairyKnockout.Condition.Value = 1;
                    else if (touhouFairyKnockout.Condition.Value == 2)
                        touhouFairyKnockout.Condition.Complete();
                }

                return true;
            }

            return null;
        }
        public override void PlayerKill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            player.MP().DaiyouseiCD = 0;
        }
    }
}
