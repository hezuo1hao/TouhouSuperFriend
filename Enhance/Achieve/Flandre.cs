using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Flandre : BaseEnhance
    {
        public override string Text => GetText("Flandre");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<FlandrePudding>());
        }
        public override void PlayerPreUpdate(Player player)
        {
        }
        public override float? PlayerUseTimeMultiplier(Player player, Item item)
        {
            if (item.pick > 0 || item.hammer > 0 || item.axe > 0 || item.type is ItemID.WireCutter)
                return 0.5f;

            return 1f;
        }
    }
}
