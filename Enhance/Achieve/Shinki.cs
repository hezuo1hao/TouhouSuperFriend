using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Shinki : BaseEnhance
    {
        public override string Text => GetText("Shinki");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override bool EnableBuffText => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<ShinkiHeart>());
        }
    }
}
