using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Hina : BaseEnhance
    {
        public override string Text => GetText("Hina");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<HinaDoll>());
        }
        public override void PlayerModifyLuck(Player player, ref float luck)
        {
            if (luck < 0)
                luck = 0;

            luck += 0.001f;
        }
    }
}
