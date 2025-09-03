using Terraria;
using Terraria.ModLoader;

namespace TouhouPetsEx.Buffs
{
    public class Throw : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.MBP().Throw = true;
        }
    }
}