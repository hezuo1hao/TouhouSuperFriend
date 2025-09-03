using Terraria;
using Terraria.ModLoader;

namespace TouhouPetsEx.Buffs
{
    public class Patience : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.MBP().Patience = true;
        }
    }
}