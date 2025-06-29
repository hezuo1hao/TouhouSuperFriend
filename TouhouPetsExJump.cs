using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Microsoft.Xna.Framework;

namespace TouhouPetsEx
{
    public class TouhouPetsExJump : ExtraJump
    {
        // 额外跳跃相对于原版的位置
        public override Position GetDefaultPosition() => BeforeBottleJumps;

        public override float GetDurationMultiplier(Player player)
        {
            // 额外跳跃持续时间
            return 1f;
        }

        public override void UpdateHorizontalSpeeds(Player player)
        {
            // 额外跳跃最大速度和加速度
            player.runAcceleration *= 2f;
            player.maxRunSpeed *= 1.25f;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
        }

        public override void ShowVisuals(Player player)
        {
            // 额外跳跃过程中的特效
            int offsetY = player.height - 6;
            if (player.gravDir == -1f)
                offsetY = 6;

            Vector2 spawnPos = new Vector2(player.position.X, player.position.Y + offsetY);

            for (int i = 0; i < 2; i++)
            {
                SpawnBlizzardDust(player, spawnPos, 0.1f, i == 0 ? -0.07f : -0.13f);
            }

            for (int i = 0; i < 3; i++)
            {
                SpawnBlizzardDust(player, spawnPos, 0.6f, 0.8f);
            }

            for (int i = 0; i < 3; i++)
            {
                SpawnBlizzardDust(player, spawnPos, 0.6f, -0.8f);
            }
        }

        private static void SpawnBlizzardDust(Player player, Vector2 spawnPos, float dustVelocityMultiplier, float playerVelocityMultiplier)
        {
            Dust dust = Dust.NewDustDirect(spawnPos, player.width, 12, Main.rand.Next([6, 59, 60, 61, 62, 63, 64, 65, 66, 75, 135, 156, 158, 169, 234, 242, 293, 294, 295, 296, 297, 298, 307, 310]), player.velocity.X * 0.3f, player.velocity.Y * 0.3f);
            dust.velocity *= dustVelocityMultiplier;
            dust.velocity += player.velocity * playerVelocityMultiplier + Vector2.UnitX * Main.rand.NextFloat(-2.00f, 2.00f);
            dust.noGravity = true;
            dust.noLight = true;
            dust.scale = 2;
        }
    }
}