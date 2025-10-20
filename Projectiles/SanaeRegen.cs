using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace TouhouPetsEx.Projectiles
{
    public class SanaeRegen : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
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
        Vector2[] p;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter.Floor() + new Vector2(0, player.gfxOffY);

            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/Regen"), Projectile.Center);
                Projectile.ai[1] = 1;

                for (int i = 0; i < 10; i++)
                {
                    var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(player.width, 0), player.width * 3, player.height * 2, DustID.IchorTorch);
                    dust.velocity = Vector2.UnitY * Main.rand.NextFloat(-12.00f, -6.00f);
                    dust.noGravity = true;
                    dust.scale = 1.5f;
                }

                p = new Vector2[8];
                for (int i = 0; i < 3; i++)
                    p[i] = new Vector2(Main.rand.NextFloat(-player.width, player.width) * Main.rand.NextFloat(2.50f, 4.00f), Main.rand.NextFloat(-player.height * .5f, player.height * 2));
                for (int i = 3; i < p.Length; i++)
                    p[i] = new Vector2(Main.rand.NextFloat(-player.width, player.width) * Main.rand.NextFloat(2.50f, 4.00f) + Main.rand.Next([-75, 75]), Main.rand.NextFloat(player.height * 2f, player.height * 4f));
            }

            if (Projectile.ai[0] > 45)
                Projectile.alpha += 17;
            else if (Projectile.ai[0] % 15 == 0)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.RainbowRodHit, new ParticleOrchestraSettings
                {
                    PositionInWorld = player.Center,
                    MovementVector = Main.rand.NextFloat(-3.140f, 3.140f).ToRotationVector2() * 20,
                });
            }

            if (Projectile.ai[0] > 60)
                Projectile.Kill();


            Projectile.ai[0]++;
        }
        public static Texture2D tex3;
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            var tex = TextureAssets.Projectile[Type].Value;
            var tex2 = TextureAssets.HotbarRadial[0].Value;
            tex3 ??= ModContent.Request<Texture2D>("TouhouPetsEx/Extra/Ex1").Value;
            var pos = Projectile.Center - Main.screenPosition;
            var a = (255 - Projectile.alpha) / 255f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos, null, Color.Yellow * a, 0, tex.Size() / 2f, 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(tex2, new Vector2(pos.X + 50, pos.Y - 10 + 100 * (float)Math.Pow(1 - Projectile.ai[0] / 50f, 2)), null, new Color(512, 512, 100) * a, 0, tex2.Size() / 2f, 0.4f, SpriteEffects.None, 0);

            List<List<VertexInfo2>> verticess = [];//声明一下顶点绘制
            List<VertexInfo2> vertices2 = [];
            List<VertexInfo2> vertices3 = [];
            float dis = 100;
            for (int k = 0; k < p.Length; k++)
            {
                if (Projectile.ai[0] <= 10 && k >= 3)
                    break;
                List<VertexInfo2> vertices = [];
                for (int i = 0; i < dis; i += 2)//遍历经过的路径
                {
                    Vector2 nV = Vector2.UnitY;
                    Vector2 routeCenter = (Projectile.Center - Projectile.ai[0] * 2 * Vector2.UnitY + p[k]) + nV * i * 1.1f - Main.screenPosition;
                    if (k >= 3)
                    {
                        routeCenter = (Projectile.Center - Projectile.ai[0] * 5 * Vector2.UnitY + p[k]) + nV * i * 1.1f - Main.screenPosition;
                        dis = 200 - k * 8;
                    }
                    //前面的二维向量需要分别填写你要绘制的起点和终点、后面的三维向量的前两个需要填写你所使用贴图的范围（0f-1f）也是起点和终点，第三个根据你写的顶点绘制文件决定效果，这里用的文件里第三个是深度（但是深度是啥？）
                    vertices.Add(new VertexInfo2(routeCenter + nV * -i * .1f + (nV.ToRotation() + MathHelper.PiOver2).ToRotationVector2() * ((dis / 2f > i) ? (20 / dis * i) : (20 - 20 / dis * i)), new Vector3(0f, 0f, 1), new Color(200, 200, 80) * a * Math.Min(Projectile.ai[0] / 20f, 1)));
                    vertices.Add(new VertexInfo2(routeCenter + nV * -i * .1f + (nV.ToRotation() - MathHelper.PiOver2).ToRotationVector2() * ((dis / 2f > i) ? (20 / dis * i) : (20 - 20 / dis * i)), new Vector3(1f, 1f, 1), new Color(200, 200, 80) * a * Math.Min(Projectile.ai[0] / 20f, 1)));
                }
                verticess.Add(vertices);
            }
            for (float r = 0; r <= 360; r += 1)
            {
                if (Projectile.ai[0] > 10)
                {
                    vertices2.Add(new VertexInfo2(Projectile.Center - Main.screenPosition + MathHelper.ToRadians(r).ToRotationVector2() * (Projectile.ai[0] * 3.5f - 35) / (1 + Projectile.ai[0] * 0.02f), new Vector3(r / 360f, 0f, 1 - r / 360f), new Color(180, 180, 80) * a));
                    vertices2.Add(new VertexInfo2(Projectile.Center - Main.screenPosition + MathHelper.ToRadians(r).ToRotationVector2() * (Projectile.ai[0] * 3.5f + 5) / (1 + Projectile.ai[0] * 0.02f), new Vector3(r / 360f, 1f, 1 - r / 360f), new Color(180, 180, 80) * a));
                }
                if (Projectile.ai[0] > 35)
                {
                    vertices3.Add(new VertexInfo2(Projectile.Center - Main.screenPosition + MathHelper.ToRadians(r).ToRotationVector2() * (Projectile.ai[0] * 2.5f - 85), new Vector3(r / 360f, 0f, 1 - r / 360f), new Color(220, 220, 120) * a));
                    vertices3.Add(new VertexInfo2(Projectile.Center - Main.screenPosition + MathHelper.ToRadians(r).ToRotationVector2() * (Projectile.ai[0] * 2.5f - 45), new Vector3(r / 360f, 1f, 1 - r / 360f), new Color(220, 220, 120) * a));
                }
            }
                //引用贴图
                Main.graphics.GraphicsDevice.Textures[0] = tex3;
            foreach (List<VertexInfo2> vertices in verticess)
                if (vertices.Count >= 3)//判断是否有三个点
                {
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                }
            if (vertices2.Count >= 3)//判断是否有三个点
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices2.ToArray(), 0, vertices2.Count - 2);
            }
            if (vertices3.Count >= 3)//判断是否有三个点
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices3.ToArray(), 0, vertices3.Count - 2);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}