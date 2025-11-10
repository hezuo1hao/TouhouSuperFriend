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
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Buffs;
using static TouhouPetsEx.TouhouPetsEx;

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
        /// <summary>
        /// 熔化debuff，灵乌路空用
        /// </summary>
        public bool Melt;
        public override bool InstancePerEntity => true;
        private static void ProcessDemonismAction(Action<BaseEnhance> action)
        {
            foreach (BaseEnhance enhance in TouhouPetsEx.GEnhanceInstances.Values)
            {
                action(enhance);
            }
        }
        /// <param name="priority">填写需要优先返回的bool结果，如：执行三次，俩false一true，需求true，则返回true结果
        /// <br>特别的，如果填null则会返回最后一个非null的结果</br>
        /// </param>
        private static bool? ProcessDemonismAction(bool? priority, Func<BaseEnhance, bool?> action)
        {
            if (priority == null)
            {
                bool? @return = null;
                foreach (BaseEnhance enhance in TouhouPetsEx.GEnhanceInstances.Values)
                {
                    bool? a = action(enhance);
                    if (a != null) @return = a;
                }
                return @return;
            }
            else
            {
                bool? @return = null;
                foreach (BaseEnhance enhance in TouhouPetsEx.GEnhanceInstances.Values)
                {
                    bool? a = action(enhance);
                    if (a == priority) return a;
                    else if (a != null) @return = a;
                }
                return @return;
            }
        }
        public override void ResetEffects(NPC npc)
        {
            Earth = false;
            MoonMist = false;
            Depression = false;
            Restless = false;
            Melt = false;
        }
        public override bool PreAI(NPC npc)
        {
            if (Earth && MoonMist && Depression && Restless && Melt && npc.HasBuff(ModContent.BuffType<LeiZhe>()) && Main.netMode != NetmodeID.Server)
                ModContent.GetInstance<SufferingFromVariousIllnesses>().Condition.Complete();

            bool? reesult = ProcessDemonismAction(false, (enhance) => enhance.NPCPreAI(npc));

            return reesult ?? base.PreAI(npc);
        }
        public override void AI(NPC npc)
        {
            ProcessDemonismAction((enhance) => enhance.NPCAI(npc));
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff(BuffID.Confused) && WorldEnableEnhance<SatoriSlippers>())
            {
                npc.lifeRegen -= 12;
                if (damage < 5) damage = 5;
            }

            if (Melt && damage < 6)
                damage = 6;
        }
        public override bool CanHitNPC(NPC npc, NPC target)
        {
            bool? reesult = ProcessDemonismAction(false, (enhance) => enhance.NPCCanHitNPC(npc, target));

            return reesult ?? base.CanHitNPC(npc, target);
        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Earth)
                modifiers.Defense /= 2f;

            if (npc.HasBuff(BuffID.Confused) && WorldEnableEnhance<SatoriSlippers>())
                modifiers.Defense /= 2f;

            if (Restless)
                modifiers.FinalDamage *= 1.05f;

            if (npc.HasBuff(ModContent.BuffType<LeiZhe>()))
                modifiers.FinalDamage *= 1 + (float)Math.Ceiling(npc.buffTime[npc.FindBuffIndex(ModContent.BuffType<LeiZhe>())] / 60f) * 0.02f;
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
        public override bool CheckDead(NPC npc)
        {
            if (Melt && Main.LocalPlayer.EnableEnhance<UtsuhoEye>())
            {
                var meltdown = ModContent.GetInstance<Meltdown>();
                meltdown.Condition.Value++;

                if (meltdown.Condition.Value == Meltdown.Max)
                    meltdown.Condition.Complete();
            }

            return base.CheckDead(npc);
        }
    }
}
