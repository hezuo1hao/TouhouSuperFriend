using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Dusts;
using TouhouPetsEx.Enhance.Achieve;

namespace TouhouPetsEx.Projectiles
{
    public class PopularityExplosionEffect : ModProjectile
    {
        public override string Texture => "TouhouPetsEx/Projectiles/DaiyouseiBoom";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 5;
        }
        bool a;
        public override void AI()
        {
            if (a)
                return;

            Player player = Main.player[Projectile.owner];

            for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / Main.rand.NextFloat(36.0f, 72.0f))
            {
                Vector2 velocity = new Vector2((float)Math.Cos(r), (float)Math.Sin(r)) * Main.rand.NextFloat(4.00f, 7.00f);
                Dust dust = Dust.NewDustDirect(player.MountedCenter, 2, 2, DustID.YellowTorch, 0, 10, 0, default, 2.4f);
                dust.noGravity = true;
                dust.velocity = velocity;
            }

            SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_powerup"), player.Center);

            a = true;
        }
    }
}