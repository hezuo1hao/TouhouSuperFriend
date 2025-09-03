using Terraria;
using Terraria.ModLoader;

namespace TouhouPetsEx.Buffs
{
    public class MystiasSong : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.MBP().Glutton = true;
            player.MBP().Patience = true;
            player.MBP().Throw = true;
        }
    }
}