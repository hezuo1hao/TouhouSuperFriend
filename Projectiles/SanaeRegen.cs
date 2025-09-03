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
                    var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(player.width, 0), player.width * 3, player.height * 2, DustID.GemTopaz);
                    dust.velocity = Vector2.UnitY * Main.rand.NextFloat(-12.00f, -6.00f);
                    dust.noGravity = true;
                    dust.scale = 1.5f;
                }

                p = new Vector2[3];
                for (int i = 0; i < p.Length; i++)
                    p[i] = new Vector2(Main.rand.NextFloat(-player.width, player.width) * Main.rand.NextFloat(2.50f, 4.00f), Main.rand.NextFloat(-player.height * .5f, player.height * 2));
            }

            if (Projectile.ai[0] > 45)
                Projectile.alpha += 17;
            else
                foreach (Vector2 pos in p)
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                    {
                        PositionInWorld = player.Bottom + pos - Projectile.ai[0] * Vector2.UnitY * 2,
                        MovementVector = -Vector2.UnitY * 12,
                        UniqueInfoPiece = (byte)(Main.rgbToHsl(Color.Yellow).X * 255f),
                    });
                }

            if (Projectile.ai[0] > 60)
                Projectile.Kill();

            Projectile.ai[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            var tex = TextureAssets.Projectile[Type].Value;
            var tex2 = TextureAssets.HotbarRadial[0].Value;
            var pos = Projectile.Center - Main.screenPosition;
            var a = (255 - Projectile.alpha) / 255f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos, null, Color.Yellow * a, 0, tex.Size() / 2f, 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(tex2, new Vector2(pos.X + 50, pos.Y - 10 + 100 * (float)Math.Pow(1 - Projectile.ai[0] / 50f, 2)), null, new Color(512, 512, 100), 0, tex2.Size() / 2f, 0.4f, SpriteEffects.None, 0);

            TouhouPetsEx.RingShader.Parameters["width"].SetValue(Math.Max(0f, 0.5f - (50f / Projectile.width)));
            TouhouPetsEx.RingShader.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, pos, null, Color.Yellow * a, 0, TextureAssets.MagicPixel.Size() /2f, new Vector2(Projectile.width, Projectile.width * 0.001f), SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}