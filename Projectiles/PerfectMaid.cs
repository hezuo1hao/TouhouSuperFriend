using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Dusts;
using TouhouPetsEx.Enhance.Achieve;

namespace TouhouPetsEx.Projectiles
{
    public class PerfectMaid : ModProjectile
    {
        public override string Texture => "TouhouPetsEx/Projectiles/DaiyouseiBoom";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 180;
        }
        bool a;
        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 20)
                Projectile.Resize((int)(15 * Projectile.ai[0]), (int)(115 * Projectile.ai[0]));
            else if (Projectile.ai[0] > 160)
                Projectile.Resize((int)(15 * (180 - Projectile.ai[0])), (int)(15 * (180- Projectile.ai[0])));


            if (!a)
            {
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/dio"), Projectile.Center);

                if (Projectile.ai[1] != -1 && Projectile.owner == Main.myPlayer)
                {
                    float dis = Main.npc[(int)Projectile.ai[1]].Size.Length();

                    for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 18f)
                    {
                        Vector2 velocity = new((float)Math.Cos(r), (float)Math.Sin(r));
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + velocity * (dis + 70), velocity * 24 / -10000f, ModContent.ProjectileType<FlyingKnife>(), (int)Projectile.ai[2], 0.5f, Projectile.owner, ai1: 18);
                    }

                    for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 9f)
                    {
                        Vector2 velocity = new((float)Math.Cos(r), (float)Math.Sin(r));
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + velocity * (dis + 20), velocity * 24 / -10000f, ModContent.ProjectileType<FlyingKnife>(), (int)Projectile.ai[2], 0.5f, Projectile.owner, ai1: 11);
                    }
                }
            }

            a = true;
        }
        RenderTarget2D render = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, TouhouPetsEx.InverseColor, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //TouhouPetsEx.RingShader.Parameters["width"].SetValue(Math.Max(0f, 0.5f - (50f / Projectile.width)));
            //TouhouPetsEx.RingShader.CurrentTechnique.Passes[0].Apply();
            //TouhouPetsEx.GrayishWhiteShader.CurrentTechnique.Passes[0].Apply();
            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, null, Color.White * ((255 - Projectile.alpha) / 255f), 0, TextureAssets.MagicPixel.Size() / 2f, new Vector2(Projectile.width, Projectile.width * 0.001f), SpriteEffects.None, 0);

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            render ??= new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            var shader = TouhouPetsEx.GrayishWhiteShader;
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
            TouhouPetsEx.RingShader.Parameters["width"].SetValue(0);
            TouhouPetsEx.RingShader.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, null, Color.White * ((255 - Projectile.alpha) / 255f), 0, TextureAssets.MagicPixel.Size() / 2f, new Vector2(Projectile.width, Projectile.width * 0.001f), SpriteEffects.None, 0);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            Main.instance.GraphicsDevice.Textures[1] = render;
            shader.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(screenTargetSwap, Vector2.Zero, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}