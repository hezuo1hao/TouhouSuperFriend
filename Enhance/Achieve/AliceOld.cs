using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class AliceOld : BaseEnhance
    {
        public override string Text => GetText("AliceOld");
        public override string[] ExperimentalText => [GetText("AliceOld_2")];
        public override bool[] Experimental => [Config.AliceOld];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<AliceOldDoll>());
        }
        public override void PlayerResetEffects(Player player)
        {
            if (player.MP().EatBook >= 100)
                player.MP().ActiveEnhanceCount += 2;
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

            player.statManaMax2 += Math.Clamp(player.MP().EatBook, 0, 100);
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (Config.AliceOld)
                player.GetDamage(DamageClass.Magic) += player.statManaMax2 / 1000f;
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (item.type == ItemID.Book)
            {
                if (player.MP().EatBook < 100 && player.EnableEnhance<AliceOldDoll>())
                {
                    item.useAnimation = item.useTime = 15;
                    item.createTile = -1;
                    item.useStyle = ItemUseStyleID.HoldUp;
                    item.UseSound = SoundID.Item4;
                    item.consumable = true;
                }
                else
                {
                    Item item1 = ContentSamples.ItemsByType[item.type];
                    item.useTime = item1.useTime;
                    item.useAnimation = item1.useAnimation;
                    item.createTile = item1.createTile;
                    item.useStyle = item1.useStyle;
                    item.UseSound = item1.UseSound;
                    item.consumable = item1.consumable;
                }
            }
        }
        public override void ItemHoldItem(Item item, Player player)
        {
            if (item.type == ItemID.Book)
            {
                if (player.MP().EatBook < 100 && player.EnableEnhance<AliceOldDoll>())
                {
                    item.useAnimation = item.useTime = 15;
                    item.createTile = -1;
                    item.useStyle = ItemUseStyleID.HoldUp;
                    item.UseSound = SoundID.Item4;
                    item.consumable = true;
                }
                else
                {
                    Item item1 = ContentSamples.ItemsByType[item.type];
                    item.useTime = item1.useTime;
                    item.useAnimation = item1.useAnimation;
                    item.createTile = item1.createTile;
                    item.useStyle = item1.useStyle;
                    item.UseSound = item1.UseSound;
                    item.consumable = item1.consumable;
                }
            }
        }
        public override bool? ItemUseItem(Item item, Player player)
        {
            if (item.type == ItemID.Book && player.MP().EatBook < 100 && player == Main.LocalPlayer)
            {
                player.MP().EatBook += 1;
                EnhancePlayers.AwardPlayerSync(TouhouPetsEx.Instance, -1, player.whoAmI);
                return true;
            }
            return null;
        }
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine = new(TouhouPetsEx.Instance, "EatBookTooltip", GetText("AliceOld_1", Main.LocalPlayer.MP().EatBook));

            if (item.type == ItemID.Book && Main.LocalPlayer.EnableEnhance<AliceOldDoll>())
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
