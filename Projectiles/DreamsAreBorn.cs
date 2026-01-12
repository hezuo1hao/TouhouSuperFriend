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
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Dusts;
using TouhouPetsEx.Enhance.Achieve;

namespace TouhouPetsEx.Projectiles
{
    public class DreamsAreBorn : ModProjectile
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

            Projectile.ai[0]++;
            Projectile.Center = player.MountedCenter.Floor() + new Vector2(0, player.gfxOffY);
            Projectile.timeLeft = 5;

            if (Projectile.ai[0] == 3600)
            {
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_cat00"), Projectile.Center);

                int time = 1800;
                player.MP().ReimuCD = true;

                player.immune = true;
                player.immuneTime += time;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += time;
                }
            }

            if (Projectile.ai[0] > 5460 || ((player.velocity.Y == 0 || !player.EnableEnhance<ReimuYinyangOrb>()) && Projectile.ai[0] < 3600))
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[Type].Value;

            for (int i = 0; i < 8; i++)
            {
                Color color = Color.White;
                Vector2 pos = Projectile.Center + (MathHelper.TwoPi / 8f * i + (float)Math.Cos(Projectile.ai[0] / 57f) * 7).ToRotationVector2() * 100 - Main.screenPosition;
                float rot = (float)Math.Sin(Projectile.ai[0] / 2100f) * 360;

                if (Projectile.ai[0] < 60)
                    color = Color.Lerp(color * 0, color, Projectile.ai[0] / 60f);

                if (Projectile.ai[0] > 5400)
                    color = color * ((5460 - Projectile.ai[0]) / 60f);

                Main.spriteBatch.Draw(tex, pos, null, color, rot, tex.Size() * .5f, 1f, SpriteEffects.None, 0);

                if (Projectile.ai[0] > (i + 1) * 450)
                    Main.spriteBatch.Draw(tex, pos, null, Color.Lerp(color, new(255, 255, 255, 0), (float)Math.Abs(Math.Cos(Main.GameUpdateCount / 31f))) * ((5460 - Projectile.ai[0]) / 60f), rot, tex.Size() * .5f, 1f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}