using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class YukariAndRanAndChen : BaseEnhance
    {
        public override string Text => GetText("YukariAndRanAndChen");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override string[] ExperimentalText => [GetText("YukariAndRanAndChen_1")];
        public override bool[] Experimental => [Config.Yukari];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<YukarisItem>());
        }
        public override void PlayerResetEffects(Player player)
        {
            if (player.MP().YukariCD > 0)
                player.MP().YukariCD--;
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.statManaMax2 += 20;
        }
    }
}
