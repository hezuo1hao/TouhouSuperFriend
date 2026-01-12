using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Security.Principal;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Dusts;
using TouhouPetsEx.Enhance.Achieve;
using static System.Net.Mime.MediaTypeNames;

namespace TouhouPetsEx.Projectiles
{
    public class YukaSpore : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.SporeTrap}";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            float num849 = 1f - Projectile.alpha / 255f;
            num849 *= Projectile.scale;
            Lighting.AddLight(Projectile.Center, 0.2f * num849, 0.275f * num849, 0.075f * num849);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 90f)
                Projectile.localAI[0] *= -1f;

            if (Projectile.localAI[0] >= 0f)
                Projectile.scale += 0.003f;
            else
                Projectile.scale -= 0.003f;

            Projectile.rotation += 0.0025f * Projectile.scale;
            float num850 = 1f;
            float num851 = 1f;
            if (Projectile.identity % 6 == 0)
                num851 *= -1f;

            if (Projectile.identity % 6 == 1)
                num850 *= -1f;

            if (Projectile.identity % 6 == 2)
            {
                num851 *= -1f;
                num850 *= -1f;
            }

            if (Projectile.identity % 6 == 3)
                num851 = 0f;

            if (Projectile.identity % 6 == 4)
                num850 = 0f;

            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 60f)
                Projectile.localAI[1] = -180f;

            if (Projectile.localAI[1] >= -60f)
            {
                Projectile.velocity.X += 0.002f * num851;
                Projectile.velocity.Y += 0.002f * num850;
            }
            else
            {
                Projectile.velocity.X -= 0.002f * num851;
                Projectile.velocity.Y -= 0.002f * num850;
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 180)
            {
                Projectile.ai[1] = 1f;
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 5;
                    if (Projectile.alpha > 255)
                        Projectile.alpha = 255;
                }
                else if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                float num852 = (Projectile.Center - Main.player[Projectile.owner].Center).Length() / 100f;
                if (num852 > 4f)
                    num852 *= 1.1f;

                if (num852 > 5f)
                    num852 *= 1.2f;

                if (num852 > 6f)
                    num852 *= 1.3f;

                if (num852 > 7f)
                    num852 *= 1.4f;

                if (num852 > 8f)
                    num852 *= 1.5f;

                if (num852 > 9f)
                    num852 *= 1.6f;

                if (num852 > 10f)
                    num852 *= 1.7f;

                Projectile.ai[0] += num852;
                if (Projectile.alpha > 50)
                {
                    Projectile.alpha -= 10;
                    if (Projectile.alpha < 50)
                        Projectile.alpha = 50;
                }
            }

            bool flag47 = false;
            Vector2 vector106 = new Vector2(0f, 0f);
            float num853 = 340f;
            for (int num854 = 0; num854 < 200; num854++)
            {
                if (Main.npc[num854].CanBeChasedBy(this))
                {
                    float num855 = Main.npc[num854].position.X + (float)(Main.npc[num854].width / 2);
                    float num856 = Main.npc[num854].position.Y + (float)(Main.npc[num854].height / 2);
                    float num857 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num855) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num856);
                    if (num857 < num853)
                    {
                        num853 = num857;
                        vector106 = Main.npc[num854].Center;
                        flag47 = true;
                    }
                }
            }

            if (flag47)
            {
                Vector2 vector107 = vector106 - Projectile.Center;
                vector107.Normalize();
                vector107 *= 4f;
                Projectile.velocity = (Projectile.velocity * 40f + vector107) / 41f;
                Projectile.ai[0] = 90;
                Projectile.ai[1] = 0f;
            }
            else if (Projectile.velocity.Length() > 0.2)
            {
                Projectile.velocity *= 0.98f;
            }
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 180 ? false : null;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 11));
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && Projectile.ai[1] == 0f)
            {
                Vector2 vector36 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                vector36.Normalize();
                vector36 *= 0.3f;
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, vector36, Main.rand.Next(569, 572), Projectile.damage, 0f, Projectile.owner);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[Projectile.ai[2] == 0 ? ProjectileID.SporeTrap : ProjectileID.SporeTrap2].Value;
            Color color = new(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}