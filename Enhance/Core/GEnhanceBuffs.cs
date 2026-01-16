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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	/// <summary>
	/// 增强相关的全局 Buff 钩子分发。
	/// <para>
	/// 主要用于：修改 Buff 文本、在 Buff Update 阶段分发增强逻辑、以及绘制额外提示信息。
	/// </para>
	/// </summary>
	public class GEnhanceBuffs : GlobalBuff
    {
        public static HashSet<int> FireDebuff;
        private static bool alreadyDrawn;
        #region 防止闭包的私有字段们
        string ModifyBuffText_buffName;
        string ModifyBuffText_tip;
        int ModifyBuffText_rare;
        #endregion
        private static void ProcessDemonismAction(Player player, Action<BaseEnhance> action)
        {
            if (!player.HasTouhouPetsBuff())
                return;

            foreach (EnhancementId enhanceId in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
            {
                if (EnhanceRegistry.TryGetEnhancement(enhanceId, out var enhancement))
                    action(enhancement);
            }
        }
        public override void Unload()
        {
            FireDebuff = null;
        }
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.LocalPlayer;

            ModifyBuffText_buffName = buffName;
            ModifyBuffText_tip = tip;
            ModifyBuffText_rare = rare;
            ProcessDemonismAction(player, (enhance) => enhance.ModifyBuffText(player, type, ref ModifyBuffText_buffName, ref ModifyBuffText_tip, ref ModifyBuffText_rare));
            buffName = ModifyBuffText_buffName;
            tip = ModifyBuffText_tip;
            rare = ModifyBuffText_rare;

            if (!LocalConfig.Tooltip_1 && !LocalConfig.Tooltip_2)
                return;

            if (BuffLoader.GetBuff(type)?.Mod.Name == "TouhouPets")
            {
                // 汇总当前玩家启用的增强（主动/被动），并拼接到 Buff 描述末尾。
                List<EnhancementId> allActiveEnhance = [];

                if (LocalConfig.Tooltip_1)
                {
                    foreach (EnhancementId id in player.MP().ActiveEnhance)
                    {
                        if (EnhanceRegistry.TryGetEnhancement(id, out var enhancement) && enhancement.EnableBuffText)
                            allActiveEnhance.Add(id);
                    }
                }

                if (LocalConfig.Tooltip_2)
                {
                    foreach (EnhancementId id in player.MP().ActivePassiveEnhance)
                    {
                        if (EnhanceRegistry.TryGetEnhancement(id, out var enhancement) && enhancement.EnableBuffText)
                            allActiveEnhance.Add(id);
                    }
                }

                HashSet<EnhancementId> bannedEnhanceIds = [];
                foreach (EnhancementId id in allActiveEnhance)
                {
                    if (!EnhanceRegistry.TryGetEnhancement(id, out var enhancement))
                        continue;

                    foreach (int itemType in enhancement.BanTootips)
                    {
                        if (EnhanceRegistry.TryGetEnhanceId(itemType, out EnhancementId bannedId))
                            bannedEnhanceIds.Add(bannedId);
                    }
                }

                allActiveEnhance.RemoveAll(id => bannedEnhanceIds.Contains(id));

                if (allActiveEnhance.Count != 0)
                    tip += "\n" + GetText("Common");
                else return;

                foreach (EnhancementId id in allActiveEnhance)
                {
                    if (!EnhanceRegistry.TryGetEnhancement(id, out var enh))
                        continue;

                    // 每个增强的基础描述。
                    tip += "\n" + enh.Text;

                    for (int i = 0; i < enh.Experimental.Length; i++)
                    {
                        if (enh.Experimental[i] && enh.ExperimentalText[i] != "")
                        {
                            tip += "\n" + enh.ExperimentalText[i];
                        }
                    }
                }
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, int type, int buffIndex, BuffDrawParams drawParams)
        {
            if (!alreadyDrawn && LocalConfig.Tooltip_3 && BuffLoader.GetBuff(type)?.Mod.Name == "TouhouPets")
            {
                // 只绘制一次“当前已启用增强数量/上限”，避免多 Buff 重复绘制。
                alreadyDrawn = true;
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, Main.LocalPlayer.MP().ActiveEnhance.Count + " / " + Main.LocalPlayer.MP().ActiveEnhanceCount, drawParams.Position.X, drawParams.Position.Y + 32, Color.AliceBlue, Color.Black, Vector2.Zero, 0.8f);
            }
        }
        public override void Update(int type, Player player, ref int buffIndex)
        {
            // 每帧重置绘制标志，让 PostDraw 在下一帧能继续工作。
            alreadyDrawn = false;

            int buffIndex2 = buffIndex;
            // 让增强有机会在 Buff Update 阶段影响 buffIndex（例如移除/替换）。
            ProcessDemonismAction(player, (enhance) => enhance.BuffUpdate(type, player, ref buffIndex2));
            buffIndex = buffIndex2;
        }
    }
}
