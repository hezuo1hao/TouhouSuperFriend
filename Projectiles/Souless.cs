using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Projectiles
{
    public class Souless : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.aiStyle = -1;
            Projectile.scale = 1;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        Vector2[] dusts = new Vector2[10];
        public override void AI()
        {
            if (Projectile.timeLeft % 3 == 0)
            {
                for (int i = dusts.Length - 1; i > 0; i--)
                    dusts[i] = dusts[i - 1];

                dusts[0] = Projectile.Center + new Vector2(Main.rand.NextFloat(-10.00f, 10.00f), Main.rand.NextFloat(-10.00f, 10.00f));
            }

            for (int i = 0; i < dusts.Length; i++)
                dusts[i].Y -= 3f;

            Projectile.velocity.Y = (float)Math.Cos(Projectile.ai[0] / 10f + Projectile.ai[1]);

            if (Projectile.ai[0] is 35 or 55)
            {
                int index = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, Main.rand.Next(3, 6));

                if (index < 600)
                {
                    if (ChildSafety.Disabled)
                        EnhanceSystem.GoreDamage[index] = Main.hardMode ? 100 : 25;
                    else
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileID.SpiritHeal, 0, 0, Projectile.owner, Projectile.owner, 1);
                }
            }

            if (Projectile.ai[0] < 15)
                Projectile.alpha -= 17;

            if (Projectile.timeLeft < 15)
                Projectile.alpha += 17;

            Projectile.ai[0]++;

            #region 切换帧图
            int frameSpeed = 4;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            #endregion
        }
        static Texture2D tex = null;
        public override bool PreDraw(ref Color lightColor)
        {
            tex ??= TextureAssets.Projectile[Type].Value;
            SpriteBatch sb = Main.spriteBatch;
            Rectangle rec = new(Projectile.frame * 56, 0, 56, 52);
            Color color = Color.White * ((255 - Projectile.alpha) / 255f);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            sb.Draw(tex, Projectile.Center - Main.screenPosition, rec, color, 0, rec.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            for (int i = 0;i < dusts.Length;i++)
            {
                Vector2 pos = dusts[i];
                sb.Draw(tex, pos - Main.screenPosition, rec, color, 0, rec.Size() / 2f, Math.Clamp(Projectile.scale * (dusts.Length - i) / dusts.Length * 0.5f, 0, int.MaxValue), SpriteEffects.None, 0);
            }

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}