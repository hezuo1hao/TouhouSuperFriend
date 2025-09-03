using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;
namespace TouhouPetsEx.Buffs
{
    public class MoonMist : ModBuff
    {
        public override string Texture => "TouhouPetsEx/Buffs/NPCDebuff";
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GEnhanceNPCs>().MoonMist = true;
            if (Main.rand.NextBool(10))
                Main.instance._ambientWindSys.SpawnAirborneCloud((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
        }
    }
}