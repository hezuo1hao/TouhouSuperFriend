using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace TouhouPetsEx.Buffs
{
    public class FragrantAromaFillsTheAir : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.Sunflower}";
        public override void Update(Player player, ref int buffIndex)
        {
            player.MBP().FragrantAromaFillsTheAir = true;
        }
    }
}