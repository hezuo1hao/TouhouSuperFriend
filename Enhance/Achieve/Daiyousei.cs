using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Buffs;
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
        public override void PlayerPreUpdateBuffsAlways(Player player)
        {
            if (!player.HasBuff<DaiyouseiCD>())
                return;

            if (player.buffTime[player.FindBuffIndex(ModContent.BuffType<DaiyouseiCD>())] <= 2)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_cardget"), player.Center);
        }
        public override void PlayerModifyHurt(Player player, ref Player.HurtModifiers modifiers)
        {
            if (!player.HasBuff<DaiyouseiCD>())
                modifiers.ModifyHurtInfo += Modifiers_ModifyHurtInfo;
        }

        private void Modifiers_ModifyHurtInfo(ref Player.HurtInfo info)
        {
            info.Dodgeable = true;
        }

        public override bool? PlayerFreeDodge(Player player, Player.HurtInfo info)
        {
            if (!player.HasBuff<DaiyouseiCD>())
            {
                int time = player.longInvince ? 240 : 180;

                player.immune = true;
                player.immuneTime += time;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += time;
                }
                player.AddBuff(ModContent.BuffType<DaiyouseiCD>(), player.DetermineCD(info, 5400));
                Projectile.NewProjectile(player.GetSource_OnHurt(info.DamageSource), player.Center, Vector2.Zero, ModContent.ProjectileType<DaiyouseiBoom>(), (info.ReflectionDamage() + 10) * 10, 5, player.whoAmI);

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
    }
}
