using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Minoriko : BaseEnhance
    {
        public override string Text => GetText("Minoriko");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MinorikoSweetPotato>());
        }
        public override void ItemHoldItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<MinorikoSweetPotato>() && !player.HasBuff(BuffID.WellFed) && !player.HasBuff(BuffID.WellFed2) && !player.HasBuff(BuffID.WellFed3))
            {
                EnhanceOn.NoUpdate = true;
                player.AddBuff(BuffID.WellFed, 36000);
                EnhanceOn.NoUpdate = false;
                SoundEngine.PlaySound(SoundID.Item2, player.Center);
            }
        }
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<MinorikoSweetPotato>())
            {
                tooltips.Insert(tooltips.FindIndex(tip => tip.Mod == "TouhouPetsEx" && tip.Name == "EnhanceTooltip") + 1, new("ExTooltip", GetText("Minoriko_0")));
            }
        }
    }
}
