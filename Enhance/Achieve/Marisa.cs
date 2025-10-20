using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Marisa : BaseEnhance
    {
        public override string Text => GetText("Marisa");
        public override bool[] Experimental => [Config.Marisa];
        public override string[] ExperimentalText => [GetText("Marisa_1")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MarisaHakkero>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Magic) *= 1.25f;

            if (Config.Marisa)
            {
                player.statManaMax2 += 100;
                player.manaRegenDelay = 0;
            }
        }
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type != ModContent.ItemType<MarisaHakkero>())
                return;

            int index = tooltips.GetTooltipsLastIndex();
            tooltips.Insert(index + 1, new TooltipLine(TouhouPetsEx.Instance, "SpecialTooltip", GetText("Marisa_0")));
        }
    }
}
