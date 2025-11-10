using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Momoyo : BaseEnhance
    {
        static private int[] Edible = [ItemID.Amethyst, ItemID.Topaz, ItemID.Sapphire, ItemID.Emerald, ItemID.Ruby, ItemID.Diamond, ItemID.Amber, ItemID.WhitePearl, ItemID.BlackPearl, ItemID.PinkPearl, ItemID.CrystalShard];
        public override string Text => GetText("Momoyo");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MomoyoPickaxe>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            EnhancePlayers plr = player.MP();

            player.moveSpeed += plr.ExtraAddition[0] / 500f;
            player.runSlowdown += plr.ExtraAddition[0] / 500f / 5f;
            player.pickSpeed -= plr.ExtraAddition[1] / 500f;
            player.breathMax += plr.ExtraAddition[2];
            player.statLifeMax2 += plr.ExtraAddition[3];
            player.lavaMax += plr.ExtraAddition[4];
            player.endurance += plr.ExtraAddition[5] / 1000f;

            if (player != Main.LocalPlayer)
                return;

            bool a = true;
            for (int i = 0; i < plr.ExtraAddition.Length; i++)
            {
                if (i is 2 or 4)
                    continue;

                if (plr.ExtraAddition[i] < EnhancePlayers.ExtraAdditionMax[i])
                    a = false;
            }

            if (a)
                ModContent.GetInstance<StartupCompleted>().Condition.Complete();
        }
        public override void PlayerModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += player.MP().ExtraAddition[6] / 1000f;
            modifiers.ScalingArmorPenetration += player.MP().ExtraAddition[10] / 500f;

            if (target.type >= 87 && target.type <= 92) modifiers.TargetDamageMultiplier *= 3;
        }
        public override void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0 && target.type >= 87 && target.type <= 92 && player.statLife < player.statLifeMax2) player.Heal(150);
        }
        public override void PlayerModifyLuck(Player player, ref float luck)
        {
            EnhancePlayers plr = player.MP();

            luck += plr.ExtraAddition[7] / 100f + plr.ExtraAddition[8] / 40f + plr.ExtraAddition[9] / 10f;
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (Edible.Contains(item.type))
            {
                int i = Array.IndexOf(Edible, item.type);
                if (player.MP().ExtraAddition[i] < EnhancePlayers.ExtraAdditionMax[i] && player.EnableEnhance<MomoyoPickaxe>())
                {
                    item.useAnimation = item.useTime = 15;
                    item.createTile = -1;
                    item.useStyle = ItemUseStyleID.DrinkOld;
                    item.UseSound = SoundID.Item2;
                    item.consumable = true;
                }
                else
                {
                    Item item1 = ContentSamples.ItemsByType[item.type];
                    item.useAnimation = item1.useAnimation;
                    item.useTime = item1.useTime;
                    item.createTile = item1.createTile;
                    item.useStyle = item1.useStyle;
                    item.UseSound = item1.UseSound;
                    item.consumable = item1.consumable;
                }
            }
        }
        public override void ItemHoldItem(Item item, Player player)
        {
            if (Edible.Contains(item.type))
            {
                int i = Array.IndexOf(Edible, item.type);
                if (player.MP().ExtraAddition[i] < EnhancePlayers.ExtraAdditionMax[i] && player.EnableEnhance<MomoyoPickaxe>())
                {
                    item.useAnimation = item.useTime = 15;
                    item.createTile = -1;
                    item.useStyle = ItemUseStyleID.DrinkOld;
                    item.UseSound = SoundID.Item2;
                    item.consumable = true;
                }
                else
                {
                    Item item1 = ContentSamples.ItemsByType[item.type];
                    item.useAnimation = item1.useAnimation;
                    item.useTime = item1.useTime;
                    item.createTile = item1.createTile;
                    item.useStyle = item1.useStyle;
                    item.UseSound = item1.UseSound;
                    item.consumable = item1.consumable;
                }
            }
        }
        public override bool? ItemUseItem(Item item, Player player)
        {
            if (Edible.Contains(item.type) && player == Main.LocalPlayer)
            {
                int i = Array.IndexOf(Edible, item.type);
                if (player.MP().ExtraAddition[i] >= EnhancePlayers.ExtraAdditionMax[i])
                    return null;
            }
            else
                return null;

            int buffType = BuffID.WellFed;

            switch (item.type)
            {
                case ItemID.Amethyst:
                    player.MP().ExtraAddition[0]++;
                    buffType = BuffID.WellFed;
                    break;

                case ItemID.Topaz:
                    player.MP().ExtraAddition[1]++;
                    buffType = BuffID.WellFed;
                    break;

                case ItemID.Sapphire:
                    player.MP().ExtraAddition[2]++;
                    buffType = BuffID.WellFed2;
                    break;

                case ItemID.Emerald:
                    player.MP().ExtraAddition[3]++;
                    buffType = BuffID.WellFed2;
                    break;

                case ItemID.Ruby:
                    player.MP().ExtraAddition[4]++;
                    buffType = BuffID.WellFed2;
                    break;

                case ItemID.Diamond:
                    player.MP().ExtraAddition[5]++;
                    buffType = BuffID.WellFed3;
                    break;

                case ItemID.Amber:
                    player.MP().ExtraAddition[6]++;
                    buffType = BuffID.WellFed2;
                    break;

                case ItemID.WhitePearl:
                    player.MP().ExtraAddition[7]++;
                    buffType = BuffID.WellFed;
                    break;

                case ItemID.BlackPearl:
                    player.MP().ExtraAddition[8]++;
                    buffType = BuffID.WellFed2;
                    break;

                case ItemID.PinkPearl:
                    player.MP().ExtraAddition[9]++;
                    buffType = BuffID.WellFed3;
                    break;

                case ItemID.CrystalShard:
                    player.MP().ExtraAddition[10]++;
                    buffType = BuffID.WellFed2;
                    break;
            }

            player.AddBuff(buffType, 28800);

            EnhancePlayers.AwardPlayerSync(TouhouPetsEx.Instance, -1, player.whoAmI);

            return true;
        }
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            EnhancePlayers plr = Main.LocalPlayer.MP();

            TooltipLine[] tooltipLine = [
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_1", GetText("Momoyo_1", plr.ExtraAddition[0] / 5f)),
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_2", GetText("Momoyo_2", plr.ExtraAddition[1] / 5f)),
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_3", GetText("Momoyo_3", plr.ExtraAddition[2])),
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_4", GetText("Momoyo_4", plr.ExtraAddition[3])),
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_5", GetText("Momoyo_5", plr.ExtraAddition[4])),
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_6", GetText("Momoyo_6", plr.ExtraAddition[5] / 10f)),
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_7", GetText("Momoyo_7", plr.ExtraAddition[6] / 10f)),
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_8", GetText("Momoyo_8", plr.ExtraAddition[7] / 100f + plr.ExtraAddition[8] / 40f + plr.ExtraAddition[9] / 10f)),
                new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_9", GetText("Momoyo_9", plr.ExtraAddition[10] / 5f))
                ];

            if (Main.LocalPlayer.EnableEnhance<MomoyoPickaxe>())
            {
                switch (item.type)
                {
                    case ItemID.Amethyst:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine[0]);
                        return;

                    case ItemID.Topaz:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine[1]);
                        return;

                    case ItemID.Sapphire:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine[2]);
                        return;

                    case ItemID.Emerald:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine[3]);
                        return;

                    case ItemID.Ruby:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine[4]);
                        return;

                    case ItemID.Diamond:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine[5]);
                        return;

                    case ItemID.Amber:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine[6]);
                        return;

                    case ItemID.WhitePearl:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_8", GetText("Momoyo_10", plr.ExtraAddition[7] / 100f)));
                        return;

                    case ItemID.BlackPearl:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_8", GetText("Momoyo_10", plr.ExtraAddition[8] / 40f)));
                        return;

                    case ItemID.PinkPearl:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, new TooltipLine(TouhouPetsEx.Instance, "ExtraAdditionTooltip_8", GetText("Momoyo_10", plr.ExtraAddition[9] / 10f)));
                        return;

                    case ItemID.CrystalShard:
                        tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, tooltipLine[8]);
                        return;
                }
            }

            if (item.type == ModContent.ItemType<MomoyoPickaxe>())
            {
                int index = tooltips.FindIndex(tooltip => tooltip.Name == "EnhanceTooltip");
                for (int i = 0; i < tooltipLine.Length; i++)
                {
                    tooltips.Insert(index + i + 1, tooltipLine[i]);
                }
            }
        }
    }
}
