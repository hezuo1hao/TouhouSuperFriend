using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	public class EnhanceSystem : ModSystem
    {
        /// <summary>
        /// ��ǰ�����ϴ��ڼ�������npc�������񡢾�����buff�ã��ϰ������������أ�
        /// </summary>
        public static int TownNPCCount;
        /// <summary>
        /// �ܹ��ж��ٸ�����npc�������񡢾�����buff�ã��ϰ������������أ�
        /// </summary>
        public static int TownNPCMax;
        private static void ProcessDemonismAction(Action<BaseEnhance> action)
        {
            foreach (BaseEnhance enhance in TouhouPetsEx.GEnhanceInstances.Values)
            {
                action(enhance);
            }
        }
        public override void PostSetupContent()
        {
            TownNPCMax = ContentSamples.NpcsByNetId.Values.Count(npc => npc.townNPC);

            ProcessDemonismAction((enhance) => enhance.SystemPostSetupContent());
        }
        public override void PostAddRecipes()
        {
            ProcessDemonismAction((enhance) => enhance.SystemPostAddRecipes());
        }
        public override void PostUpdateNPCs()
        {
            TownNPCCount = 0;

            if (!Main.dayTime && (Main.bloodMoon || Main.GetMoonPhase() == Terraria.Enums.MoonPhase.Full))
                TownNPCCount = TownNPCMax;
            else foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.townNPC)
                        TownNPCCount++;
                }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            ProcessDemonismAction((enhance) => enhance.SystemPostUpdateNPCs());
        }
        public override void PostUpdateEverything()
        {
            ProcessDemonismAction((enhance) => enhance.SystemPostUpdateEverything());
        }
    }
}
