using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
namespace TouhouPetsEx.Buffs
{
    public class ChenCD : CDBuff
    {
        public override string BuffName => "YukariBuff";
        public override bool IsLoadingEnabled(Mod mod) => true;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
    }
}