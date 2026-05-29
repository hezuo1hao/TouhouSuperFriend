using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Security.Principal;
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
    public class CriticalHit : ModProjectile
    {
        public override string Texture => "TouhouPetsEx/Projectiles/DaiyouseiBoom";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 100;
        }
        bool a;
        public override void AI()
        {
            if (a)
                return;
            SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/CriticalHit") with { MaxInstances = 114514 }, Projectile.Center);

            a = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[12].Value;
            var tex2 = TextureAssets.Projectile[79].Value;
            var spriteBatch = Main.spriteBatch;
            var pos = Projectile.Center - Main.screenPosition;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 7; i++)
            {
                float angle = (i * MathHelper.TwoPi / 7) - MathHelper.PiOver2;
                float x = pos.X + (float)Math.Cos(angle) * 1;
                float y = pos.Y + (float)Math.Sin(angle) * 1;

                // 主星（指向外）
                spriteBatch.Draw(tex2, new Vector2(x, y), null, Color.White,
                    angle + MathHelper.PiOver4, // 让四角星的一个角精准指向外
                    tex2.Size() / 2f,
                    0.8f, SpriteEffects.None, 0f);

                // 辅助星（稍小，旋转45度，增加星光效果）
                spriteBatch.Draw(tex2, new Vector2(x, y), null, Color.White,
                    angle, // 不同角度
                    tex2.Size() / 2f,
                    0.5f, SpriteEffects.None, 0f);
            }

            // 绘制内圈的七个内凹点（小四角星）
            float innerRadius = 1 * 0.42f;
            for (int i = 0; i < 7; i++)
            {
                float angle = (i * MathHelper.TwoPi / 7) - MathHelper.PiOver2 + MathHelper.Pi / 7;
                float x = pos.X + (float)Math.Cos(angle) * innerRadius;
                float y = pos.Y + (float)Math.Sin(angle) * innerRadius;

                spriteBatch.Draw(tex2, new Vector2(x, y), null, Color.White,
                    angle + MathHelper.PiOver4,
                    tex2.Size() / 2f,
                    0.44f, SpriteEffects.None, 0f);
            }

            // 中心星
            spriteBatch.Draw(tex2, pos, null, Color.White,
                0f, tex2.Size() / 2f,
                1.1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}