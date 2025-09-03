using Terraria;
using Terraria.ModLoader;

namespace TouhouPetsEx.Buffs
{
    public class Glutton : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.MBP().Glutton = true;
        }
    }
}