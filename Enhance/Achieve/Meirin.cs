using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Meirin : BaseEnhance
    {
        public override string Text => GetText("Meirin");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MeirinPanda>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.GetJumpState<TouhouPetsExJump>().Enable();
        }
    }
}
