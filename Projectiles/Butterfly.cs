using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Dusts;
using TouhouPetsEx.Enhance.Achieve;

namespace TouhouPetsEx.Projectiles
{
    public class Butterfly : ModProjectile
    {
        public override string Texture => "TouhouPetsEx/Projectiles/DaiyouseiBoom";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 5;
        }
        bool a;
        public override void AI()
        {
            if (a || LocalConfig.Yuyuko == YuyukoEffect.Disabled)
                return;

            if (Projectile.ai[0] == 0)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/AtkWhite") with { MaxInstances = 114514, Pitch = -0.125f }, Projectile.Center);

            if (Projectile.ai[0] == 1)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/AtkWhite") with { MaxInstances = 114514 }, Projectile.Center);

            for (int i = Main.rand.Next(8, 16); i > 0; i--)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ButterflyDust>());
                dust.fadeIn = 30;
                dust.scale = 1;
            }

            a = true;
        }
    }
}