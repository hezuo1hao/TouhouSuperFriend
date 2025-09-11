using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Tenshi : BaseEnhance
    {
        public override string Text => GetText("Tenshi");
        public override bool[] Experimental => [Config.Tenshi];
        public override string[] ExperimentalText => [GetText("Tenshi_1")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<TenshiKeyStone>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.statDefense += 4;
        }
    }
}
