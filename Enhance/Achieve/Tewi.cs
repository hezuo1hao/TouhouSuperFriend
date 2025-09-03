using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Tewi : BaseEnhance
    {
        public override string Text => GetText("Tewi");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<TewiCarrot>());
        }
        public override void PlayerModifyLuck(Player player, ref float luck)
        {
            luck += 0.4f;
        }
    }
}
