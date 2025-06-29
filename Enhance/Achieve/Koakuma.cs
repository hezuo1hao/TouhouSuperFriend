using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Koakuma : BaseEnhance
    {
        public override string Text => GetText("Koakuma");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<KoakumaPower>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.GetAttackSpeed(DamageClass.Generic) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 5;
        }
    }
}
