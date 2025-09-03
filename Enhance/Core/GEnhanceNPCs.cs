using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
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
        /// <summary>
        /// 月雾debuff，帕秋莉用
        /// </summary>
        public bool MoonMist;
        /// <summary>
        /// 忧郁debuff，露娜萨用
        /// </summary>
        public bool Depression;
        /// <summary>
        /// 躁动ddebuff，梅露兰用
        /// </summary>
        public bool Restless;
        /// <summary>
        /// 超级暴击，饕餮Buff用（米斯蒂娅能力相关）
        /// </summary>
        public bool SuperCrit;
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
            MoonMist = false;
            Depression = false;
            Restless = false;
        }
        public override void AI(NPC npc)
        {
            ProcessDemonismAction(npc, (enhance) => enhance.NPCAI(npc));
        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Earth)
                modifiers.Defense /= 2f;

            if (Restless)
                modifiers.FinalDamage *= 1.05f;
        }
        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Depression)
                modifiers.FinalDamage *= 0.9f;
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (Depression)
                modifiers.FinalDamage *= 0.9f;
        }
    }
}
