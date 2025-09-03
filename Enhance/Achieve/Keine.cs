using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Keine : BaseEnhance
    {
        public override string Text => GetText("Keine");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<KeineLeaf>());
        }
        public override void PlayerResetEffects(Player player)
        {
            if (player.MP().KeineCD[0] > 0)
                player.MP().KeineCD[0]--;
        }
        public override void PlayerPostUpdateBuffs(Player player)
        {
            if (player.MP().KeineCD[0] == 0)
            {
                List<int> buffs = [ModContent.BuffType<Sword>(), ModContent.BuffType<Jade>(), ModContent.BuffType<Mirror>(), ModContent.BuffType<Township>()];
                buffs.Remove(player.MP().KeineCD[1]);
                player.MP().KeineCD[1] = Main.rand.Next(buffs);
                player.AddBuff(player.MP().KeineCD[1], 1800);
                player.MP().KeineCD[0] = 1800;
            }
        }
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type != ModContent.ItemType<KeineLeaf>() || Main.dayTime || (!Main.bloodMoon && Main.GetMoonPhase() != Terraria.Enums.MoonPhase.Full))
                return;

            tooltips.Insert(tooltips.FindIndex(tooltips => tooltips.Mod == "TouhouPetsEx" && tooltips.Name == "EnhanceTooltip") + 1, new(TouhouPetsEx.Instance, "EnhanceTooltip1", GetText("Keine_1")));
        }
    }
}
