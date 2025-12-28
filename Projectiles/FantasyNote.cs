using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Achieve;

namespace TouhouPetsEx.Projectiles
{
    public class FantasyNote : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.QuarterNote}";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.light = 0.3f;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.localNPCHitCooldown = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 33;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 1)
                Projectile.width = 18;
            else if (Projectile.ai[0] == 2)
                Projectile.width = 22;

            Projectile.ai[1]++;
            Projectile.spriteDirection = -Projectile.direction;

            if (Projectile.ai[1] < 120)
                return;

            if (Math.Abs(Projectile.velocity.Y) < Projectile.ai[2])
                Projectile.velocity.Y += Projectile.ai[2] / 100f * Math.Sign(Projectile.velocity.Y);

            Projectile.rotation = Projectile.velocity.X * 0.1f;

            if (Projectile.ai[1] > 360)
                Projectile.Kill();
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[1] >= 120;
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[1] >= 120 ? null : false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
            {
                Projectile.velocity.X = oldVelocity.X * -1;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -1;
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 80, default, 1.5f).noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.ai[0] switch
            {
                0 => TextureAssets.Projectile[ProjectileID.QuarterNote].Value,
                1 => TextureAssets.Projectile[ProjectileID.EighthNote].Value,
                2 => TextureAssets.Projectile[ProjectileID.TiedEighthNote].Value,
                _ => TextureAssets.Projectile[ProjectileID.QuarterNote].Value
            };
            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.09f, 0.018f));

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(200, 200, 200, 0) * ((255 - Projectile.alpha) / 255f), Projectile.rotation, tex.Size() / 2f, Projectile.scale, (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }
    }
}