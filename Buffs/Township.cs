using Terraria;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Buffs
{
    public class Township : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += EnhanceSystem.TownNPCCount / 100f;
            player.statDefense += EnhanceSystem.TownNPCCount;
            player.statLifeMax2 += EnhanceSystem.TownNPCCount * 10;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += GetText("Buffs.Township.Description_1", EnhanceSystem.TownNPCCount, EnhanceSystem.TownNPCCount * 10);
        }
    }
}