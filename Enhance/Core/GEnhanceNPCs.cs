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
    /// <summary>
    /// 增强相关的全局 NPC 钩子分发与状态承载（按实体实例化）。
    /// <para>
    /// 一部分增强需要在 NPC 上挂载临时状态（例如多种 debuff 标记、超级暴击标记等），这里统一存放。
    /// </para>
    /// </summary>
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
        /// 是否为灵，阿燐-灵者杀手用
        /// </summary>
        public static HashSet<int> Spirit = [NPCID.Ghost, NPCID.DungeonSpirit, NPCID.FloatyGross, NPCID.Wraith, NPCID.ShadowFlameApparition, NPCID.PirateGhost, NPCID.Poltergeist];
        /// <summary>
        /// 熔化debuff，灵乌路空用
        /// </summary>
        public bool Melt;
        /// <summary>
        /// 每个 NPC 实例拥有独立的 <see cref="GlobalNPC"/> 数据。
        /// </summary>
        public override bool InstancePerEntity => true;
        public override void Unload()
        {
            Spirit = null;
        }
        public override void ResetEffects(NPC npc)
        {
            // 每 tick 重置“瞬时状态”标记，避免跨 tick 残留。
            Earth = false;
            MoonMist = false;
            Depression = false;
            Restless = false;
            Melt = false;
        }
        public override bool PreAI(NPC npc)
        {
            // 成就：同时满足多种 debuff 且带雷蛰 buff。
            if (Earth && MoonMist && Depression && Restless && Melt && npc.HasBuff(ModContent.BuffType<LeiZhe>()) && Main.netMode != NetmodeID.Server)
                ModContent.GetInstance<SufferingFromVariousIllnesses>().Condition.Complete();

            bool? reesult = null;
            // 只遍历覆写了 NPCPreAI 的增强，减少空调用。
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
            // AI 为 void：不需要聚合返回值，直接分发即可。
            foreach (BaseEnhance enhance in EnhanceHookRegistry.NPCAI)
                enhance.NPCAI(npc);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            // 小五：混乱目标额外掉血（世界存在该增强才生效）。
            if (npc.HasBuff(BuffID.Confused) && WorldEnableEnhance<SatoriSlippers>())
            {
                npc.lifeRegen -= 12;
                if (damage < 5) damage = 5;
            }

            // 熔化：最低 DoT 伤害底线。
            if (Melt && damage < 6)
                damage = 6;
        }
        public override bool CanHitNPC(NPC npc, NPC target)
        {
            bool? reesult = null;
            // 只遍历覆写了 NPCCanHitNPC 的增强，并按“false 优先短路”的策略聚合。
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
            // 多种 debuff 对伤害/防御的调整集中在这里。
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
            // 忧郁：降低输出伤害。
            if (Depression)
                modifiers.FinalDamage *= 0.9f;
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            // 忧郁：降低输出伤害（打玩家同样生效）。
            if (Depression)
                modifiers.FinalDamage *= 0.9f;
        }
        public override bool CheckDead(NPC npc)
        {
            // 熔化击杀统计（阿空相关成就）。
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
