using Terraria;
using Terraria.ModLoader;

namespace TouhouPetsEx.Dusts
{
    public class DepressionDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new(0, 14 * Main.rand.Next(2), 20, 12);
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            if (dust.alpha > 0 && dust.fadeIn > 0)
            {
                dust.alpha -= 15;
            }
            else if (dust.fadeIn > 0)
            {
                dust.fadeIn--;
            }
            else
            {
                dust.alpha += 10;
            }

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}