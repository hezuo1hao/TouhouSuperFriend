using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    /// <summary>
    /// 这个能力主要起到占位作用，具体实现在 <see cref="Sunny"/>、<see cref="Luna"/>、<see cref="TouhouPetsExMapLayer"/> 里
    /// </summary>
    public class SAYYA : BaseEnhance
    {
        public override string Text => GetText("SAYYA");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<LightsJewels>());
            AddBanTootips([ModContent.ItemType<SunnyMilk>(), ModContent.ItemType<LunaMoon>(), ModContent.ItemType<StarSapphire>()]);
        }
    }
}
