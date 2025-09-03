using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using ReLogic.Utilities;
using Terraria.ID;

namespace TouhouPetsEx.Projectiles
{
    public class YukaEffects : ModProjectile
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
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter.Floor() + new Vector2(0, player.gfxOffY);

            if (Projectile.ai[0] == 0)
                Projectile.localAI[0] = SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/sun"), Projectile.Center).ToFloat();

            if (Projectile.ai[0] > 240)
                Projectile.alpha += 9;

            if (Projectile.ai[0] > 270)
                Projectile.Kill();

            if (SoundEngine.TryGetActiveSound(SlotId.FromFloat(Projectile.localAI[0]), out ActiveSound activeSound))
            {
                activeSound.Position = Projectile.Center;
            }

            Projectile.ai[0]++;
        }
        public static Texture2D tex = null;
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            tex ??= TextureAssets.Projectile[Type].Value;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(180, 180, 0) * ((255 - Projectile.alpha) / 255f), Projectile.ai[0] / 9f, tex.Size() / 2f, Projectile.ai[0] / 100f, SpriteEffects.None, 0);
            if (Projectile.ai[0] > 135)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(120, 120, 0) * ((255 - Projectile.alpha) / 255f), Projectile.ai[0] / -11f, tex.Size() / 2f, (Projectile.ai[0] - 135) / 60f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}