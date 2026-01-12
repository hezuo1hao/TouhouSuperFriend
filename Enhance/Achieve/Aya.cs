using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Aya : BaseEnhance
    {
        public override string Text => GetText("Aya");
        public override string[] ExperimentalText => [GetText("Aya_1")];
        public override bool[] Experimental => [Config.Aya];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<AyaCamera>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.moveSpeed += 0.35f;
            player.accRunSpeed += 0.35f;
            player.runSlowdown += 0.35f;

            if ((player.direction == 1 && Main.windSpeedCurrent > 0) || (player.direction == -1 && Main.windSpeedCurrent < 0))
            {
                player.maxRunSpeed += 2.5f * Math.Abs(Main.windSpeedCurrent);
                player.accRunSpeed += 2.5f * Math.Abs(Main.windSpeedCurrent);
            }
        }
        public override void ItemHorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            if (Config.Aya && player.velocity.Y != 0)
                player.maxRunSpeed *= 2;
        }
        public override void PlayerPostUpdateRunSpeeds(Player player)
        {
            if (Config.Aya && player.velocity.Y != 0)
                player.maxRunSpeed *= 2;
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (!player.ZoneSkyHeight || player != Main.LocalPlayer)
                return;

            var NovaDrift = ModContent.GetInstance<NovaDrift>();

            if (NovaDrift.IsCloneable)
                return;

            Vector2 vector = player.velocity + player.instantMovementAccumulatedThisFrame;
            if (player.mount.Active && player.mount.IsConsideredASlimeMount && player.velocity.Y != 0f && !player.SlimeDontHyperJump)
                vector.Y += player.velocity.Y;

            int num15 = (int)(1f + vector.Length() * 6f);
            if (num15 > player.speedSlice.Length)
                num15 = player.speedSlice.Length;

            float num16 = 0f;
            for (int num17 = num15 - 1; num17 > 0; num17--)
            {
                player.speedSlice[num17] = player.speedSlice[num17 - 1];
            }

            player.speedSlice[0] = vector.Length();
            for (int m = 0; m < player.speedSlice.Length; m++)
            {
                if (m < num15)
                    num16 += player.speedSlice[m];
                else
                    player.speedSlice[m] = num16 / (float)num15;
            }

            num16 /= (float)num15;
            int num18 = 42240;
            int num19 = 216000;
            float num20 = num16 * (float)num19 / (float)num18;
            if (!player.merman && !player.ignoreWater)
            {
                if (player.honeyWet)
                    num20 /= 4f;
                else if (player.wet)
                    num20 /= 2f;
            }

            if (NovaDrift.Condition.Value < num20)
                NovaDrift.Condition.Value = num20;

            if (num20 >= NovaDrift.Max)
                NovaDrift.Condition.Complete();
        }
    }
}
