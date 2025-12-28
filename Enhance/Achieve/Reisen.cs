using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Reisen : BaseEnhance
    {
        public override string Text => GetText("Reisen");
        public override bool[] Experimental => [Config.Reisen];
        public override string[] ExperimentalText => [GetText("Reisen_1")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<ReisenGun>());
        }
        public override void PlayerModifyHurt(Player player, ref Player.HurtModifiers modifiers)
        {
            player.MP().ReisenDodge = false;

            if (Main.rand.Next(100) < 17)
            {
                player.MP().ReisenDodge = true;
                modifiers.ModifyHurtInfo += Modifiers_ModifyHurtInfo;
            }
        }

        private void Modifiers_ModifyHurtInfo(ref Player.HurtInfo info)
        {
            info.Dodgeable = true;
        }

        public override bool? PlayerFreeDodge(Player player, Player.HurtInfo info)
        {
            if (player.MP().ReisenDodge)
            {
                player.MP().ReisenDodge = false;
                int time = player.longInvince ? 120 : 80;
                player.immune = true;
                player.immuneTime += time;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += time;
                }

                Projectile.NewProjectile(player.GetSource_Misc("Dodge"), player.Center, Vector2.Zero, ModContent.ProjectileType<ReisenEffect>(), 0, 0, player.whoAmI);
                return true;
            }

            return null;
        }
    }
}
