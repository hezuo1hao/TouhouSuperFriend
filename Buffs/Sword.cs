using Terraria;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Buffs
{
    public class Sword : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += EnhanceSystem.TownNPCCount / 100f;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += GetText("Buffs.Sword.Description_1", EnhanceSystem.TownNPCCount);
        }
    }
}