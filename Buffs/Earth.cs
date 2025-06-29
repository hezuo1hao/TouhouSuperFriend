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
            Dust dust = Dust.NewDustDirect(npc.Center, 2, 2, DustID.Dirt, 10, 10);
            dust.velocity = new Vector2(Main.rand.NextFloat(-1.00f, 1.00f), Main.rand.NextFloat(-1.00f, 1.00f));
            dust.scale = Main.rand.NextFloat(2.00f, 3.00f);
        }
    }
}