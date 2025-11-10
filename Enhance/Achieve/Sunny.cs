using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Sunny : BaseEnhance
    {
        public override string Text => GetText("Sunny");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SunnyMilk>());
        }
        public override void SystemModifyLightingBrightness(ref float scale)
        {
            if (Main.LocalPlayer.EnableEnhance<SunnyMilk>() || Main.LocalPlayer.EnableEnhance<LightsJewels>())
                scale *= 1.03f;
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            if (player == Main.LocalPlayer && player.EnableEnhance<SunnyMilk>() && player.EnableEnhance<LunaMoon>() && player.EnableEnhance<SunnyMilk>() && player.EnableEnhance<StarSapphire>() && player.EnableEnhance<LightsJewels>())
                ModContent.GetInstance<MathClassroom>().Condition.Complete();
        }
    }
}
