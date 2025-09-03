using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Aya : BaseEnhance
    {
        public override string Text => GetText("Aya");
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
    }
}
