using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;
namespace TouhouPetsEx.Buffs
{
    public class Melt : ModBuff
    {
        public override string Texture => "TouhouPetsEx/Buffs/NPCDebuff";
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= (int)Math.Ceiling(npc.buffTime[buffIndex] / 50f);

            if (Main.hardMode)
                npc.lifeRegen -= (int)Math.Ceiling(npc.buffTime[buffIndex] / 20f);

            if (npc.TryGetGlobalNPC<GEnhanceNPCs>(out var gnpc))
                gnpc.Melt = true;

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Lava);
                dust.velocity = new Vector2(Main.rand.NextFloat(-1.00f, 1.00f), Main.rand.NextFloat(2.00f, 4.00f));
                dust.scale = Main.rand.NextFloat(1.00f, 1.50f);
                dust.noGravity = true;
            }

            npc.buffTime[buffIndex] = 0;
            npc.buffType[buffIndex] = 0;
            for (int i = 0; i < NPC.maxBuffs - 1; i++)
            {
                if (Config.Utsuho_3 && GEnhanceBuffs.FireDebuff.Contains(npc.buffType[i]))
                    npc.lifeRegen -= 9;

                if (npc.buffTime[i] == 0 || npc.buffType[i] == 0)
                {
                    for (int j = i + 1; j < NPC.maxBuffs; j++)
                    {
                        npc.buffTime[j - 1] = npc.buffTime[j];
                        npc.buffType[j - 1] = npc.buffType[j];
                        npc.buffTime[j] = 0;
                        npc.buffType[j] = 0;
                    }
                }
            }
        }
    }
}