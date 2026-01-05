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

            bool? reesult = null;
            foreach (BaseEnhance enhance in EnhanceHookRegistry.NPCPreAI)
            {
                bool? a = enhance.NPCPreAI(npc);
                if (a == false)
                    return false;
                if (a != null)
                    reesult = a;
            }

            return reesult ?? base.PreAI(npc);
        }
        public override void AI(NPC npc)
        {
            foreach (BaseEnhance enhance in EnhanceHookRegistry.NPCAI)
                enhance.NPCAI(npc);
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
            bool? reesult = null;
            foreach (BaseEnhance enhance in EnhanceHookRegistry.NPCCanHitNPC)
            {
                bool? a = enhance.NPCCanHitNPC(npc, target);
                if (a == false)
                    return false;
                if (a != null)
                    reesult = a;
            }

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
