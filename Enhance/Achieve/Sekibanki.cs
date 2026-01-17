using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Sekibanki : BaseEnhance
    {
        public override string Text => GetText("Sekibanki");
        public override string[] ExperimentalText => [GetText("Sekibanki_1")];
        public override bool[] Experimental => [Config.Sekibanki];
        public override bool Passive => true;
        public override bool EnableBuffText => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SekibankiBow>());
        }
        public override void ItemHoldItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<SekibankiBow>())
                player.remoteVisionForDrone = true;
        }
        public override bool? ItemCanUseItem(Item item, Player player, ref bool def)
        {
            if (player.altFunctionUse == 2 && player.whoAmI == Main.myPlayer && item.type == ModContent.ItemType<SekibankiBow>())
            {
                foreach (Projectile proj in Main.ActiveProjectiles)
                    if (proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<PlayerHead>())
                    {
                        (proj.ModProjectile as PlayerHead).homing++;
                        proj.netUpdate = true;
                    }

                if (player.ownedProjectileCounts[ModContent.ProjectileType<PlayerHead>()] == 0)
                {
                    Vector2 pos = player.Center - Vector2.UnitY * 12;
                    Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(item, 0), pos, Vector2.Normalize(Main.MouseWorld - pos) * 3, ModContent.ProjectileType<PlayerHead>(), 0, 0, player.whoAmI, ai2: -2);
                }

                return false;
            }

            return null;
        }
    }
}
