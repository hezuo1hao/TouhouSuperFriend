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
        /// 当前世界上存在几个城镇npc，剑、玉、镜、乡buff用（上白泽慧音能力相关）
        /// </summary>
        public static int TownNPCCount;
        /// <summary>
        /// 总共有多少个城镇npc，剑、玉、镜、乡buff用（上白泽慧音能力相关）
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
