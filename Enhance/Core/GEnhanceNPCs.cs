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
        /// <summary>
        /// 赫卡提亚+皮丝用
        /// </summary>
        public int TorchDamage;
        /// <summary>
        /// 土debuff，帕秋莉用
        /// </summary>
        public bool Earth;
        public override bool InstancePerEntity => true;
        private static void ProcessDemonismAction(NPC npc, Action<BaseEnhance> action)
        {
            foreach (BaseEnhance enhance in TouhouPetsEx.GEnhanceInstances.Values)
            {
                action(enhance);
            }
        }
        public override void ResetEffects(NPC npc)
        {
            Earth = false;
        }
        public override void AI(NPC npc)
        {
            ProcessDemonismAction(npc, (enhance) => enhance.NPCAI(npc));
        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Earth)
                modifiers.ScalingArmorPenetration += 0.5f;
        }
    }
}
