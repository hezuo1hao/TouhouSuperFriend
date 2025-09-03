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
        /// �տ�����+Ƥ˿��
        /// </summary>
        public int TorchDamage;
        /// <summary>
        /// ��debuff����������
        /// </summary>
        public bool Earth;
        /// <summary>
        /// ����debuff����������
        /// </summary>
        public bool MoonMist;
        /// <summary>
        /// ����debuff��¶������
        /// </summary>
        public bool Depression;
        /// <summary>
        /// �궯ddebuff��÷¶����
        /// </summary>
        public bool Restless;
        /// <summary>
        /// ��������������Buff�ã���˹���������أ�
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
