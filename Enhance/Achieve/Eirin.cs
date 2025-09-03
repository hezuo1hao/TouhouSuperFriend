using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Eirin : BaseEnhance
    {
        public override string Text => GetText("Eirin");
        public override bool EnableRightClick => false;
        public override bool Passive => true;
        public override bool[] Experimental => [Config.Eirin];
        public override string[] ExperimentalText => [GetText("Eirin_1")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<EirinBow>());
        }
        public override void SystemPostAddRecipes()
        {
            Recipe.IngredientQuantityCallback callback = new(delegate (Recipe recipe, int type, ref int amount, bool isDecrafting)
            {
                if (isDecrafting || !Main.LocalPlayer.EnableEnhance<EirinBow>())
                        return;

                for (int i = amount; i > 0; i--)
                {
                    if (Main.rand.Next(100) < 81)
                        amount--;
                }
            });

            foreach (Recipe recipe in Main.recipe)
            {
                recipe.AddConsumeIngredientCallback(callback);
            }
        }
        public override void PlayerGetHealLife(Player player, Item item, bool quickHeal, ref int healValue)
        {
            if (Config.Eirin)
                healValue = (int)(healValue * 1.25f);
        }
        public override void PlayerGetHealMana(Player player, Item item, bool quickHeal, ref int healValue)
        {
            if (Config.Eirin)
                healValue = (int)(healValue * 1.25f);
        }
    }
}
