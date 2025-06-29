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
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.Center.X > Projectile.Center.X ? 1 : -1;
            modifiers.FinalDamage /= Projectile.ai[1];
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1]++;
        }
        static Effect shader = ModContent.Request<Effect>("TouhouPetsEx/Effects/ReverseColor", AssetRequestMode.ImmediateLoad).Value;
        static Texture2D tex = ModContent.Request<Texture2D>("TouhouPetsEx/Extra/Ex1", AssetRequestMode.ImmediateLoad).Value;
        static RenderTarget2D render = null;
        public override bool PreDraw(ref Color lightColor)
        {
            tex ??= TextureAssets.Projectile[Type].Value;
            render ??= new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            var spriteBatch = Main.spriteBatch;
            var graphicsDevice = Main.instance.GraphicsDevice;
            var screenTarget = Main.screenTarget;
            var screenTargetSwap = Main.screenTargetSwap;
            var dPosition = Projectile.Center - Main.screenPosition;
            var scale = Projectile.width / 2f;
            var color = Color.White * ((255 - Projectile.alpha) / 255f);

            graphicsDevice.SetRenderTarget(screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(render);
            graphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);
            List<VertexInfo2> vertices = [];
            for (float r = 0; r <= 360; r += 1)
            {
                vertices.Add(new VertexInfo2(dPosition + MathHelper.ToRadians(r).ToRotationVector2() * scale, new Vector3(r / 360f, 0f, 1 - r / 360f), color));
                vertices.Add(new VertexInfo2(dPosition + MathHelper.ToRadians(r).ToRotationVector2() * (scale + 50f), new Vector3(r / 360f, 1f, 1 - r / 360f), color));
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = tex;
            if (vertices.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            graphicsDevice.Textures[0] = screenTargetSwap;
            graphicsDevice.Textures[1] = render;
            shader.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(screenTargetSwap, Vector2.Zero, Color.White);

            graphicsDevice.Textures[0] = null;
            graphicsDevice.Textures[1] = null;

            return false;
        }
    }
}