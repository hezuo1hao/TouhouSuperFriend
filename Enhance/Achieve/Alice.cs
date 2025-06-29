using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Alice : BaseEnhance
    {
        public override string Text => GetText("Alice");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<AliceDoll>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.maxMinions += 1;
        }
    }
}
