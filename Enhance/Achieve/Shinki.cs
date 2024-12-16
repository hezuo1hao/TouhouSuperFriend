using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Shinki : BaseEnhance
    {
        public override string Text => TouhouPetsExUtils.GetText("Shinki");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<ShinkiHeart>());
        }
    }
}
