using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Achieve;

namespace TouhouPetsEx.Projectiles
{
    public class FlyingKnife : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        NPC target = null;
        int hit;
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.spriteDirection = -Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Projectile.ai[0] == Projectile.ai[1])
                Projectile.alpha = 0;

            if (Projectile.ai[0] < 60)
                return;

            if (Projectile.ai[0] == 60)
                Projectile.velocity *= 10000;

            if (Projectile.ai[0] == 122)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.Center,
                    MovementVector = Vector2.Normalize(Projectile.velocity) * 1.5f,
                    UniqueInfoPiece = 160
                });
            }

            if (target != null && Projectile.ai[0] % 30 == 0)
            {
                Projectile.Center = target.Center;
                Projectile.localNPCImmunity[target.whoAmI] = 0;
                hit++;
            }

            if (hit > 4)
                Projectile.Kill();
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] <= 120;
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] <= 120 || target != null ? null : false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return this.target == null || this.target.whoAmI == target.whoAmI ? null : false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (this.hit == 0)
            {
                Projectile.ai[0] = 121;
                Projectile.netUpdate = true;
                this.target = target;
                this.hit++;
            }
        }
        int rgb;
        Vector2 offset;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] > 120)
                return false;

            if (rgb == 19)
                rgb = 0;

            if ((rgb == 0 && Main.rand.NextBool(15)) || rgb > 0)
                rgb++;

            var tex = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (rgb > 0)
            {
                if ((rgb - 1) % 6 == 0)
                    offset = new Vector2(Main.rand.NextFloat(-4.00f, 4.00f), Main.rand.NextFloat(-4.00f, 4.00f));

                Color color = rgb switch
                {
                    > 0 and <= 6 => new Color(255, 40, 40, 100),
                    > 6 and <= 12 => new Color(40, 255, 40, 100),
                    _ => new Color(40, 40, 255, 100)
                };
                color *= ((255 - Projectile.alpha) / 255f);

                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + offset, null, color, Projectile.rotation, tex.Size() / 2f, Projectile.scale, (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * ((255 - Projectile.alpha) / 255f), Projectile.rotation, tex.Size() / 2f, Projectile.scale, (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}