using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class AliceOld : BaseEnhance
    {
        public override string Text => TouhouPetsExUtils.GetText("AliceOld");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<AliceOldDoll>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            foreach (Item item in player.inventory)
            {
                if (item.type == ItemID.SpellTome)
                    player.statManaMax2 += item.stack / 100;
            }

            foreach (Item item in player.bank4.item)
            {
                if (item.type == ItemID.SpellTome)
                    player.statManaMax2 += item.stack / 100;
            }

            player.statManaMax2 += player.MP().EatBook;
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (item.type == ItemID.Book)
            {
                if (player.MP().ActiveEnhance.Contains(ModContent.ItemType<AliceOldDoll>()) && player.MP().EatBook < 100)
                {
                    item.useAnimation = item.useTime = 30;
                    item.useStyle = ItemUseStyleID.HoldUp;
                    item.UseSound = SoundID.Item4;
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
            Main.NewText(player.MP().EatBook);
            if (item.type == ItemID.Book && player.MP().EatBook < 100)
            {
                player.MP().EatBook += 1;
            }
            return true;
        }
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine = new TooltipLine(TouhouPetsEx.Instance, "EatBookTooltip", TouhouPetsExUtils.GetText("AliceOld_1", Main.LocalPlayer.MP().EatBook));

            if (item.type == ItemID.Book && Main.LocalPlayer.MP().ActiveEnhance.Contains(ModContent.ItemType<AliceOldDoll>()))
            {
                tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine);
            }

            if (item.type == ModContent.ItemType<AliceOldDoll>())
            {
                tooltips.Insert(tooltips.FindIndex(tooltip => tooltip.Name == "EnhanceTooltip") + 1, tooltipLine);
            }
        }
    }
}
