using Terraria;
using Terraria.ModLoader;

namespace TouhouPetsEx.Dusts
{
    public class ButterflyDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new(0, 0, 43, 36);
        }
        public override bool Update(Dust dust)
        {
            if (dust.fadeIn % 3 == 0)
                dust.frame.Y += 36;

            if (dust.frame.Y > 36 * 3)
                dust.frame.Y = 0;

            dust.position += dust.velocity;
            dust.rotation = dust.velocity.ToRotation() + 1.57f;

            if (dust.fadeIn < 0)
            {
                dust.alpha += 5;
            }

            dust.fadeIn--;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}