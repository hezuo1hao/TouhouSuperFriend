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
    public class ReisenEffect : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_slash"), Projectile.Center);

            if (Projectile.ai[0] > 20)
                Projectile.alpha += 25;

            if (Projectile.ai[0] > 30)
                Projectile.Kill();

            Projectile.ai[0]++;
        }
        Texture2D tex;
        RenderTarget2D render = null;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 1)
                return false;

            tex ??= TextureAssets.Projectile[Type].Value;
            render ??= new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            var shader = TouhouPetsEx.DistortShader;
            var spriteBatch = Main.spriteBatch;
            var graphicsDevice = Main.instance.GraphicsDevice;
            var screenTarget = Main.screenTarget;
            var screenTargetSwap = Main.screenTargetSwap;
            var dPosition = Projectile.Center - Main.screenPosition;

            graphicsDevice.SetRenderTarget(screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(render);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(tex, dPosition, null, Color.White * ((255 - Projectile.alpha) / 255f), 0, tex.Size() / 2f, Projectile.ai[0] * Projectile.ai[0] / 300f, SpriteEffects.None, 0);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            shader.Parameters["tex0"].SetValue(render);
            shader.Parameters["mult"].SetValue(0.02f);
            shader.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(screenTargetSwap, Vector2.Zero, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}