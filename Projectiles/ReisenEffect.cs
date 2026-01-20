using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.Enums;

namespace TouhouPetsEx.Projectiles
{
    public class ReisenEffect : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_slash"), Projectile.Center);

            if (Projectile.ai[0] > 20)
                Projectile.alpha += 25;

            if (Projectile.ai[0] > 30)
                Projectile.Kill();

            Projectile.ai[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}