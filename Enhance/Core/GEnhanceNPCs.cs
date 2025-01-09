using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	public class GEnhanceNPCs : GlobalNPC
    {
        private static void ProcessDemonismAction(NPC npc, Player player, Action<BaseEnhance> action)
        {
            foreach (int id in player.MP().ActiveEnhance)
            {
                action(TouhouPetsEx.GEnhanceInstances[id]);
            }
        }
        public override void AI(NPC npc)
        {
            ProcessDemonismAction(npc, Main.LocalPlayer, (enhance) => enhance.NPCAI(npc, Main.LocalPlayer));
        }
    }
}
