using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TouhouPetsEx.Projectiles
{
    public class Die : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }
        public override void AI()
        {
            if (LocalConfig.Yuyuko == YuyukoEffect.Disabled)
                return;

            if (Projectile.ai[0] > 30)
                Projectile.alpha += 25;

            if (Projectile.ai[0] == 30)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/die") with { MaxInstances = 114514 }, Projectile.Center);

            if (Projectile.alpha > 255)
                Projectile.Kill();

            Projectile.ai[0]++;
        }
        public static Asset<Texture2D> tex2;
        public override bool PreDraw(ref Color lightColor)
        {
            if (LocalConfig.Yuyuko == YuyukoEffect.Disabled)
                return false;

            var tex = TextureAssets.Projectile[Type].Value;
            tex2 ??= ModContent.Request<Texture2D>("TouhouPetsEx/Extra/Ex1");
            int interval = tex.Height / 2;
            Vector2 pos = Vector2.UnitY * interval / 4f;
            Vector2 pos2 = Math.Max(0, Projectile.ai[0] - 30) * Projectile.ai[1].ToRotationVector2() * 2;
            Color color = Color.White * ((255 - Projectile.alpha) / 255f);
            Rectangle rec = new(0, 0, tex.Width, interval);
            Rectangle rec2 = new(0, interval, tex.Width, interval);

            Main.spriteBatch.End();//结束绘制
                                   //开始绘制
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(tex, Projectile.Center - pos - pos2 - Main.screenPosition, rec, color, 0, rec.Size() / 2f, .5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + pos + pos2 - Main.screenPosition, rec2, color, 0, rec2.Size() / 2f, .5f, SpriteEffects.None, 0);

            if (Projectile.ai[0] <= 30)
                return false;

            Main.spriteBatch.End();//结束绘制
                                   //开始绘制
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<VertexInfo2> vertices = [];//声明一下顶点绘制
            List<VertexInfo2> vertices2 = [];//声明一下顶点绘制
            float dis = 200;
            for (int i = 0; i < dis; i += 4)//遍历经过的路径
            {
                Vector2 nV = Vector2.UnitX;
                Vector2 routeCenter = (Projectile.Center - Vector2.UnitX * 100) + nV * i * 1.1f;
                //前面的二维向量需要分别填写你要绘制的起点和终点、后面的三维向量的前两个需要填写你所使用贴图的范围（0f-1f）也是起点和终点，第三个根据你写的顶点绘制文件决定效果，这里用的文件里第三个是深度（但是深度是啥？）
                vertices.Add(new VertexInfo2(routeCenter - Main.screenPosition + nV * -i * .1f + (nV.ToRotation() + MathHelper.PiOver2).ToRotationVector2() * ((dis / 2f > i) ? (80 / dis * i) : (80 - 80 / dis * i)) * (255 - Projectile.alpha) / 255f, new Vector3(0f, 0f, 1), Color.DarkRed));
                vertices.Add(new VertexInfo2(routeCenter - Main.screenPosition + nV * -i * .1f + (nV.ToRotation() - MathHelper.PiOver2).ToRotationVector2() * ((dis / 2f > i) ? (80 / dis * i) : (80 - 80 / dis * i)) * (255 - Projectile.alpha) / 255f, new Vector3(1f, 1f, 1), Color.DarkRed));
                vertices2.Add(new VertexInfo2(routeCenter - Main.screenPosition + nV * -i * .1f + (nV.ToRotation() + MathHelper.PiOver2).ToRotationVector2() * ((dis / 2f > i) ? (30 / dis * i) : (30 - 30 / dis * i)) * (255 - Projectile.alpha) / 255f, new Vector3(0f, 0f, 1), Color.Red));
                vertices2.Add(new VertexInfo2(routeCenter - Main.screenPosition + nV * -i * .1f + (nV.ToRotation() - MathHelper.PiOver2).ToRotationVector2() * ((dis / 2f > i) ? (30 / dis * i) : (30 - 30 / dis * i)) * (255 - Projectile.alpha) / 255f, new Vector3(1f, 1f, 1), Color.Red));
            }
            //引用贴图
            Main.graphics.GraphicsDevice.Textures[0] = tex2.Value;
            if (vertices.Count >= 3)//判断是否有三个点
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
            }
            if (vertices2.Count >= 3)//判断是否有三个点
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices2.ToArray(), 0, vertices2.Count - 2);
            }
            Main.spriteBatch.End();//结束绘制
                                   //开始绘制
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void Unload()
        {
            tex2 = null;
        }
    }
}