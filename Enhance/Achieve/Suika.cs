using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Suika : BaseEnhance
    {
        public override string Text => GetText("Suika");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SuikaGourd>());
        }
        public override void PlayerModifyItemScale(Player player, Item item, ref float scale)
        {
            scale += .2f;
        }
        public override void ModifyBuffText(Player player, int type, ref string buffName, ref string tip, ref int rare)
        {
            if (type == BuffID.Tipsy)
                tip = GetText("Tipsy");
        }
        public override void BuffUpdate(int type, Player player, ref int buffIndex)
        {
            if (type == BuffID.Tipsy)
            {
                player.statDefense += 4;
                player.GetDamage(DamageClass.Melee) += 0.02f;
                player.GetAttackSpeed(DamageClass.Melee) += 0.02f;
                player.GetCritChance(DamageClass.Melee) += 3;
            }
        }
    }
}
