using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using TouhouPetsEx.Enhance.Achieve;
using TouhouPetsEx.Achievements;

namespace TouhouPetsEx.Projectiles
{
    public class CirnoIce : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.IceBlock}";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.coldDamage = true;
        }
        public override void AI()
        {
            Projectile.ai[1] = -1;

            if (Projectile.ai[2] <= 0)
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.type == Type && proj.whoAmI != Projectile.whoAmI && Projectile.ai[1] != proj.whoAmI && proj.Center != Projectile.Center && Collision.CheckAABBvAABBCollision(proj.Center, proj.Size, Projectile.Center, Projectile.Size))
                    {
                        float speed = (proj.velocity.Length() + Projectile.velocity.Length()) / 2f;
                        if (Config.Cirno) speed *= 1.1f;
                        proj.velocity = Vector2.Normalize(proj.Center - Projectile.Center) * speed;
                        Projectile.velocity = Vector2.Normalize(Projectile.Center - proj.Center) * speed;
                        Projectile.ai[1] = proj.whoAmI;
                    }
                }

            Projectile.ai[2]--;

            if (Projectile.lavaWet || Projectile.velocity.Length() < 0.1f || Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.Kill();

            Projectile.velocity.X *= (Projectile.ai[0] == 1) ? 0.995f : 0.99f;
            Projectile.velocity.Y += (!Projectile.wet) ? 0.12f : -0.2f;

            if (Projectile.ai[0] == 0)
                Projectile.rotation += Projectile.velocity.X * 0.2f;
            else
                Projectile.rotation -= Projectile.rotation % MathHelper.PiOver2;

            if (Main.myPlayer == Projectile.owner)
            {
                var FTL = ModContent.GetInstance<FasterThanLight>();

                if (!FTL.IsCloneable)
                {
                    if (FTL.Condition.Value < Projectile.velocity.Length())
                        FTL.Condition.Value = Projectile.velocity.Length();

                    if (Projectile.velocity.Length() >= FasterThanLight.LightSpeed)
                        FTL.Condition.Complete();
                }
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 1;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, 0f, 0f, 100, Color.White, 1f);
            }
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        }
    }
}