using Microsoft.Xna.Framework;
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
using Terraria.ModLoader.IO;
using static TouhouPetsEx.TouhouPetsEx;

namespace TouhouPetsEx.Enhance.Core
{
	/// <summary>
	/// Buff 侧状态与行为（<see cref="ModPlayer"/>）。
	/// <para>
	/// 主要用于“由 Buff 产生的临时效果”，例如米斯蒂娅相关的饕餮/耐心/投掷等状态。
	/// </para>
	/// </summary>
	public class EnhanceBuffPlayers : ModPlayer
    {
        /// <summary>
        /// 饕餮Buff用（米斯蒂娅能力相关）
        /// </summary>
        public bool Glutton = false;
        /// <summary>
        /// 耐心Buff用（米斯蒂娅能力相关）
        /// </summary>
        public bool Patience = false;
        /// <summary>
        /// 投掷Buff用（米斯蒂娅能力相关）
        /// </summary>
        public bool Throw = false;
        /// <summary>
        /// 米斯蒂娅用
        /// </summary>
        public float? OldLifeRegenTime = null;
        public override void ResetEffects()
        {
            // Patience 未激活时不需要保留旧的 lifeRegenTime 参考值。
            if (!Patience)
                OldLifeRegenTime = null;

            // Buff 标记每 tick 重新计算，默认先清空。
            Glutton = false;
            Patience = false;
            Throw = false;
        }
        public override void PreUpdate()
        {
            if (Patience)
            {
                // lifeRegenTime 可能被其它机制降低；耐心 Buff 希望锁定为“上一次的更大值”。
                if (OldLifeRegenTime != null && OldLifeRegenTime > Player.lifeRegenTime)
                    Player.lifeRegenTime = OldLifeRegenTime.Value;

                // 记录本 tick 的值供下 tick 比较。
                OldLifeRegenTime = Player.lifeRegenTime;
            }
        }
        public override void UpdateLifeRegen()
        {
            if (Patience && OldLifeRegenTime != null && OldLifeRegenTime > Player.lifeRegenTime)
            {
                // 在真正结算生命回复前再次兜底，防止流血等效果让 lifeRegenTime 掉下去。
                Player.lifeRegenTime = OldLifeRegenTime.Value;
                // 取消流血（流血会阻止生命回复）。
                Player.bleed = false;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 在命中信息最终落地前插入“超级暴击”判定。
            modifiers.ModifyHitInfo += (ref NPC.HitInfo a) => SuperCrit(ref a, target);
        }

        /// <summary>
        /// 饕餮 Buff 的“超级暴击”：在原暴击基础上小概率再次翻倍，并同步给其他客户端显示效果。
        /// </summary>
        private void SuperCrit(ref NPC.HitInfo info, NPC npc)
        {
            if (info.Crit && Player.MBP().Glutton && Main.rand.NextBool(25))
            {
                // 伤害翻倍（在暴击结算后再次加成）。
                info.Damage *= 2;

                // 客户端战斗文字/效果相关：不显示战斗文字/秒杀/隐藏伤害时，不做视觉同步。
                if (!info.HideCombatText && !info.InstantKill && npc.lifeMax > 1 && !npc.HideStrikeDamage)
                {
                    npc.GetGlobalNPC<GEnhanceNPCs>().SuperCrit = true;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return;

                    ModPacket packet = Mod.GetPacket();

                    packet.Write((byte)MessageType.SuperCrit);
                    packet.Write(npc.whoAmI);
                    packet.Send(-1, Player.whoAmI);
                }
            }
        }
    }
}
