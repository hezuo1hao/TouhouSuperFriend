using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Dusts;
using TouhouPetsEx.Enhance.Core;
namespace TouhouPetsEx.Buffs
{
    public class Depression : ModBuff
    {
        public override string Texture => "TouhouPetsEx/Buffs/NPCDebuff";
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GEnhanceNPCs>().Depression = true;
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<DepressionDust>());
                dust.fadeIn = 30;
                dust.velocity *= 0.1f;
                dust.alpha = 255;
            }
        }
    }
}