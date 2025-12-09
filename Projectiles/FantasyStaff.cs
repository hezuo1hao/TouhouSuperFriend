using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TouhouPetsEx.Enhance.Achieve;

namespace TouhouPetsEx.Projectiles
{
    public class FantasyStaff : ModProjectile
    {
        public override string Texture => "TouhouPetsEx/Extra/Ex1";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 600;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
        }
        Vector2[] P = new Vector2[3];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float movementSpeed = 0.012f;

            if (Projectile.ai[0] == 0)
            {
                P[0] = player.Center;
                P[1] = player.Center + new Vector2(-500 * Projectile.direction, -100 + Projectile.ai[1] * Projectile.ai[2]);
                P[2] = player.Center + new Vector2(100 * Projectile.direction, 25 + Projectile.ai[1] * Projectile.ai[2]);
                Projectile.netUpdate = true;
            }

            var i = movementSpeed * Projectile.ai[0];
            var A = Vector2.Lerp(P[0], P[1], i);
            var B = Vector2.Lerp(P[1], P[2], i);
            if (i < 1)
                Projectile.velocity = Vector2.Lerp(A, B, i) - Projectile.Center;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0]++;

            if (i > 1.9f)
                Projectile.alpha += 14;
            else if (i > 0.65f && player == Main.LocalPlayer && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height) && Main.rand.NextBool(30))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - Vector2.UnitY * 8, new(Main.rand.NextFloat(-1.00f, 1.00f), 0.01f), ModContent.ProjectileType<FantasyNote>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.Next(3), ai2: Main.rand.NextFloat(6.00f, 12.00f));
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            P[0] = reader.ReadVector2();
            P[1] = reader.ReadVector2();
            P[2] = reader.ReadVector2();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(P[0]);
            writer.WriteVector2(P[1]);
            writer.WriteVector2(P[2]);
        }
        Asset<Texture2D> tex2;
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> vertices = [];
            var tex = TextureAssets.Projectile[ModContent.ProjectileType<YukaEffects>()].Value;
            tex2 ??= ModContent.Request<Texture2D>("TouhouPetsEx/Extra/NoBlackPointLight");
            Color color = Color.White * ((255 - Projectile.alpha) / 255f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] != Vector2.Zero && i + 1 < Projectile.ai[0])
                {
                    float size = MathHelper.Clamp(i * 1.5f, 0, 15);

                    vertices.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2() * size, new Vector3(0, 0f, 1 - i / 360f), color));
                    vertices.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition - (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2() * size, new Vector3(1, 1, 1 - i / 360f), color));
                }
                else break;
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Type].Value;
            if (vertices.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
            }

            if (Projectile.ai[1] is 1 or 2 && Projectile.ai[0] - 2 > 0 && Projectile.oldPos[(int)Projectile.ai[0] - 2] != Vector2.Zero)
                Main.spriteBatch.Draw(tex, Projectile.oldPos[(int)Projectile.ai[0] - 2] - Main.screenPosition, null, color, Projectile.ai[0] * 0.02f * (Projectile.ai[1] == 1 ? 1 : -1), tex.Size() / 2f, 0.4f, SpriteEffects.None, 0);

            if (Projectile.ai[0] > 43)
                Main.spriteBatch.Draw(tex2.Value, Projectile.oldPos[(int)Projectile.ai[0] - 43] - Main.screenPosition, null, color, 0, tex2.Size() / 2f, MathHelper.Clamp((Projectile.ai[0] - 43) * 0.04f, 0, 0.4f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void Load()
        {
            tex2 = ModContent.Request<Texture2D>("TouhouPetsEx/Extra/NoBlackPointLight");
        }
        public override void Unload()
        {
            tex2 = null;
        }
    }
}