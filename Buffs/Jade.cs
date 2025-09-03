using Terraria;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Buffs
{
    public class Jade : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += EnhanceSystem.TownNPCCount;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += GetText("Buffs.Jade.Description_1", EnhanceSystem.TownNPCCount);
        }
    }
}