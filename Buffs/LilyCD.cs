using Terraria;
using Terraria.ModLoader;
namespace TouhouPetsEx.Buffs
{
    public class LilyCD : CDBuff
    {
        public override string BuffName => "LilyBuff";
        public override bool IsLoadingEnabled(Mod mod) => true;
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.townNPCs > 2 && !NPC.AnyDanger())
                player.buffTime[buffIndex] -= 3;
        }
    }
}