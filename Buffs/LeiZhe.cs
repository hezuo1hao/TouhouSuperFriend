using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;
namespace TouhouPetsEx.Buffs
{
    public class LeiZhe : ModBuff
    {
        public override string Texture => "TouhouPetsEx/Buffs/NPCDebuff";
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.buffTime[buffIndex] += 1;

            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Firework_Yellow, 10, 10);
                dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                dust.scale = Main.rand.NextFloat(1.00f, 1.50f);
                dust.noGravity = true;
            }
        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            if (npc.buffTime[buffIndex] < 600)
                npc.buffTime[buffIndex] += time;
            return false;
        }
    }
}