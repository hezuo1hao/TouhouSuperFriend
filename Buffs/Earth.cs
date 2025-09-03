using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;
namespace TouhouPetsEx.Buffs
{
    public class Earth : ModBuff
    {
        public override string Texture => "TouhouPetsEx/Buffs/NPCDebuff";
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GEnhanceNPCs>().Earth = true;
            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Dirt, 10, 10);
                dust.velocity = Vector2.Zero;
                dust.scale = Main.rand.NextFloat(1.00f, 1.50f);
            }
        }
    }
}