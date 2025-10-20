using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Murasa : BaseEnhance
    {
        public override string Text => GetText("Murasa");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MurasaBailer>());
        }
        public override void PlayerModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.wet)
                modifiers.FinalDamage *= 1.12f;

            if (target.dripping)
                modifiers.FinalDamage *= 1.06f;
        }
        public override float? PlayerUseTimeMultiplier(Player player, Item item)
        {
            if (item.type == ItemID.WaterGun)
                return 0.1f;

            return null;
        }
    }
}
