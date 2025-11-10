using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TouhouPetsEx.Projectiles
{
    public class Lightning : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 270;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 25;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ArmorPenetration = 2100000000;
        }
        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 9)
            {
                Projectile.alpha -= 30;
            }
            if (Projectile.timeLeft < 15)
            {
                Projectile.alpha += 17;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(tex, new Rectangle((int)(Projectile.Center.X - Main.screenPosition.X), (int)(Projectile.Center.Y - Main.screenPosition.Y), tex.Width,
                (int)(tex.Height * Main.screenHeight / 270f * 1.1f)), null, Color.White * ((255 - Projectile.alpha) / 255f), Projectile.rotation, tex.Size() / 2, SpriteEffects.None, 0);
            return false;
        }
    }
}