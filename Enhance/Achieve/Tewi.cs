using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
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
        public override void PlayerPostUpdateEquips(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                var veryLucky = ModContent.GetInstance<VeryLucky>();

                if (veryLucky.Condition.Value < player.luck)
                    veryLucky.Condition.Value = player.luck;

                if (veryLucky.Condition.Value >= VeryLucky.Max)
                    veryLucky.Condition.Complete();
            }
        }
    }
}
