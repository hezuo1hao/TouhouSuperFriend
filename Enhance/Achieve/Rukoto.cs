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
using TouhouPetsEx.Achievements;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Rukoto : BaseEnhance
    {
        public override string Text => GetText("Rukoto");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
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
                    foreach(Projectile proj in Main.ActiveProjectiles)
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
                if (rukotoSweeping)
                {
                    range *= 2;

                    if (player == Main.LocalPlayer)
                        ModContent.GetInstance<GreatPurge>().Condition.Complete();
                }

                Utils.PlotTileLine(player.Center + Vector2.UnitX * range, player.Center - Vector2.UnitX * range, range * 2, CutTiles);

                DelegateMethods.tileCutIgnore = null;
            }
        }
        bool CutTiles(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 1))
                return false;

            if (Main.tile[x, y] == null)
                return false;

            if (!Main.tileCut[Main.tile[x, y].type] && !TileID.Sets.CrackedBricks[Main.tile[x, y].type])
                return true;

            if (DelegateMethods.tileCutIgnore[Main.tile[x, y].type])
                return true;

            if (WorldGen.CanCutTile(x, y, DelegateMethods.tilecut_0) || TileID.Sets.CrackedBricks[Main.tile[x, y].type])
            {
                WorldGen.KillTile(x, y);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
            }

            return true;
        }
    }
}
