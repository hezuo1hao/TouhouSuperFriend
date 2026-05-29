using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Projectiles
{
    public class Sandevistan : ModProjectile
    {
        public override string Texture => "TouhouPetsEx/Projectiles/DaiyouseiBoom";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
        }
        int has;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] > 0)
            {
                Projectile.timeLeft = (int)Projectile.ai[0];
                Projectile.ai[0] = -1;
                Projectile.ai[1] = 13;
                player.MP().OldPlayer = [];
                player.MP().SandevistanColor = [];
            }
            NewEntityShadowInfo shadowInfo = new();
            shadowInfo.CopyPlayer(player);
            if (player.MP().OldPlayer.Count == 0 || shadowInfo.Position.Distance(player.MP().OldPlayer[has].Position) > 30)
            {
                player.MP().OldPlayer.Add(shadowInfo);
                has = player.MP().OldPlayer.Count - 1;
                Projectile.ai[1]++;
            }

            Color color = Main.hslToRgb(MathHelper.Lerp(0, 1, Projectile.ai[1] % 37 / 37f), 1f, 0.5f);
            player.MP().SandevistanColor.Add(color);
        }
    }
}