using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.Enums;

namespace TouhouPetsEx.Projectiles
{
    public class DaiyouseiBoom : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.Resize((int)(30 * Projectile.ai[0]), (int)(30 * Projectile.ai[0]));

            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_cat00"), Projectile.Center);
                Projectile.ai[1] = 1;
            }

            if (Projectile.ai[0] > 45)
                Projectile.alpha += 17;

            if (Projectile.ai[0] > 60)
                Projectile.Kill();

            Projectile.ai[0]++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // 1. 获取圆的中心点和半径
            Vector2 circleCenter = projHitbox.Center();
            float circleRadius = projHitbox.Width / 2f;

            // 2. 计算矩形中离圆心最近的点
            float closestX = Math.Clamp(circleCenter.X, targetHitbox.Left, targetHitbox.Right);
            float closestY = Math.Clamp(circleCenter.Y, targetHitbox.Top, targetHitbox.Bottom);

            // 3. 计算最近点与圆心的距离
            float distanceX = circleCenter.X - closestX;
            float distanceY = circleCenter.Y - closestY;
            float distanceSquared = distanceX * distanceX + distanceY * distanceY;

            // 4. 判断是否相交
            return distanceSquared < (circleRadius * circleRadius);
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.Center.X > Projectile.Center.X ? 1 : -1;
            modifiers.FinalDamage /= Projectile.ai[1];
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, TouhouPetsEx.InverseColor, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            TouhouPetsEx.RingShader.Parameters["width"].SetValue(Math.Max(0f, 0.5f - (50f / Projectile.width)));
            TouhouPetsEx.RingShader.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, null, Color.White * ((255 - Projectile.alpha) / 255f), 0, TextureAssets.MagicPixel.Size() /2f, new Vector2(Projectile.width, Projectile.width * 0.001f), SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}