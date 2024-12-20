using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Momoyo : BaseEnhance
    {
        public override string Text => TouhouPetsExUtils.GetText("Momoyo");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MomoyoPickaxe>());
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (item.type is ItemID.Sapphire or ItemID.Ruby or ItemID.Emerald or ItemID.Topaz or ItemID.Amethyst or ItemID.Diamond or ItemID.Amber or ItemID.)
            {
                if (player.MP().ActiveEnhance.Contains(ModContent.ItemType<AliceOldDoll>()) && player.MP().EatBook < 100)
                {
                    item.useAnimation = item.useTime = 30;
                    item.useStyle = ItemUseStyleID.DrinkOld;
                    item.UseSound = SoundID.Item2;
                    item.consumable = true;
                }
                else
                {
                    item.useAnimation = item.useTime = 0;
                    item.useStyle = ItemUseStyleID.None;
                    item.UseSound = null;
                    item.consumable = false;
                }
            }
        }
        public override bool? ItemUseItem(Item item, Player player)
        {
            return base.ItemUseItem(item, player);
        }
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ItemModifyTooltips(item, tooltips);
        }
    }
}
