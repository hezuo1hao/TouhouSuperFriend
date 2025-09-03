using Terraria;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Buffs
{
    public class Mirror : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statLifeMax2 += EnhanceSystem.TownNPCCount * 10;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += GetText("Buffs.Mirror.Description_1", EnhanceSystem.TownNPCCount * 10);
        }
    }
}