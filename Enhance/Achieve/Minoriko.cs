using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Minoriko : BaseEnhance
    {
        public override string Text => GetText("Minoriko");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MinorikoSweetPotato>());
        }
        public override void BuffUpdate(int type, Player player, ref int buffIndex)
        {
            if (type == BuffID.WellFed)
            {
                player.statDefense += 2;
                player.GetCritChance(DamageClass.Generic) += 2;
                player.GetDamage(DamageClass.Generic) += 0.05f;
                player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
                player.minionKB += 0.5f;
                player.moveSpeed += 0.2f;
                player.pickSpeed -= 0.05f;
            }

            if (type == BuffID.WellFed2)
            {
                player.statDefense += 3;
                player.GetCritChance(DamageClass.Generic) += 3;
                player.GetDamage(DamageClass.Generic) += 0.075f;
                player.GetAttackSpeed(DamageClass.Melee) += 0.075f;
                player.minionKB += 0.75f;
                player.moveSpeed += 0.3f;
                player.pickSpeed -= 0.1f;
            }

            if (type == BuffID.WellFed3)
            {
                player.statDefense += 4;
                player.GetCritChance(DamageClass.Generic) += 4;
                player.GetDamage(DamageClass.Generic) += 0.1f;
                player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
                player.minionKB += 1f;
                player.moveSpeed += 0.4f;
                player.pickSpeed -= 0.15f;
            }
        }
    }
}
