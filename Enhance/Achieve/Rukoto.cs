using Microsoft.Xna.Framework;
using System;
using Pets = TouhouPets.Content.Projectiles.Pets;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Rukoto : BaseEnhance
    {
        public override string Text => TouhouPetsExUtils.GetText("Rukoto");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<RukotoRemote>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (Main.GameUpdateCount % 60 == 0)
            {
                bool rukotoSweeping = false;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<Pets.Rukoto>()] > 0)
                {
                    foreach(Projectile proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<Pets.Rukoto>())
                        {
                            if (proj.ai[1] == 2) rukotoSweeping = true;

                            break;
                        }
                    }
                }

                DelegateMethods.tileCutIgnore = TileID.Sets.TileCutIgnore.None;

                float range = 240;
                if (rukotoSweeping) range *= 2;

                Utils.PlotTileLine(player.Center + Vector2.UnitX * range, player.Center - Vector2.UnitX * range, range * 2, DelegateMethods.CutTiles);

                DelegateMethods.tileCutIgnore = null;
            }
        }
    }
}
