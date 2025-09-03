using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TouhouPetsEx.Projectiles
{
    public class LilyDodgeEffects : ModProjectile
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

            foreach (int buffType in player.buffType)
            {
                if (Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                    player.ClearBuff(buffType);
            }
            player.breath = player.breathMax;

            if (Projectile.ai[0]  == 0)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_border"), Projectile.Center);

            if (Projectile.ai[0] > 360)
                Projectile.Kill();

            Projectile.ai[0]++;
        }
        public static Texture2D tex = ModContent.Request<Texture2D>("TouhouPetsEx/Projectiles/LilyDodgeEffects", AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D tex2 = ModContent.Request<Texture2D>("TouhouPetsEx/Projectiles/LilyDodgeEffects_1", AssetRequestMode.ImmediateLoad).Value;
        public override bool PreDraw(ref Color lightColor)
        {
            Color pink = new(255, 174, 200, 0);
            Color white = new(255, 255, 255, 0);

            if (Projectile.ai[0] > 345)
            {
                pink = new Color(255, 174, 200, 0) * ((360 - Projectile.ai[0]) / 15f);
                white = new Color(255, 255, 255, 0) * ((360 - Projectile.ai[0]) / 15f);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(pink, white, Projectile.ai[0] / 300f), Projectile.ai[0] / 11f, tex.Size() / 2f, Projectile.ai[0] / 360f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.Lerp(white, pink, Projectile.ai[0] / 300f), Projectile.ai[0] / -17f, tex2.Size() / 2f, Projectile.ai[0] / 360f, SpriteEffects.None, 0);
            return false;
        }
    }
}