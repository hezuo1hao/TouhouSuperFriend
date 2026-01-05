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
using TouhouPetsEx.Dusts;
using TouhouPetsEx.Enhance.Achieve;

namespace TouhouPetsEx.Projectiles
{
    public class PerfectMaid : ModProjectile
    {
        public override string Texture => "TouhouPetsEx/Projectiles/DaiyouseiBoom";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 180;
        }
        bool a;
        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 20)
                Projectile.Resize((int)(15 * Projectile.ai[0]), (int)(115 * Projectile.ai[0]));
            else if (Projectile.ai[0] > 160)
                Projectile.Resize((int)(15 * (180 - Projectile.ai[0])), (int)(15 * (180- Projectile.ai[0])));


            if (!a)
            {
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/dio"), Projectile.Center);

                if (Projectile.ai[1] != -1 && Projectile.owner == Main.myPlayer)
                {
                    float dis = Main.npc[(int)Projectile.ai[1]].Size.Length();

                    for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 18f)
                    {
                        Vector2 velocity = new((float)Math.Cos(r), (float)Math.Sin(r));
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + velocity * (dis + 70), velocity * 24 / -10000f, ModContent.ProjectileType<FlyingKnife>(), (int)Projectile.ai[2], 0.5f, Projectile.owner, ai1: 18);
                    }

                    for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 9f)
                    {
                        Vector2 velocity = new((float)Math.Cos(r), (float)Math.Sin(r));
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + velocity * (dis + 20), velocity * 24 / -10000f, ModContent.ProjectileType<FlyingKnife>(), (int)Projectile.ai[2], 0.5f, Projectile.owner, ai1: 11);
                    }
                }
            }

            a = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}