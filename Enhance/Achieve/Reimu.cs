using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Reimu : BaseEnhance
    {
        public override string Text => GetText("Reimu");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<ReimuYinyangOrb>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.wingTimeMax *= 2;

            if (NPC.downedEmpressOfLight)
            {
                player.empressBrooch = true;
                player.moveSpeed += 0.075f;

                if (player == Main.LocalPlayer && player.wingsLogic > 0 && player.velocity.Y < 0)
                    ModContent.GetInstance<FlyMyWings>().Condition.Complete();
            }
        }
    }
}
