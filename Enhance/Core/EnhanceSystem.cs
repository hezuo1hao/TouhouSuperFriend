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
	/// <summary>
	/// 增强系统级分发入口（<see cref="ModSystem"/>）。
	/// <para>
	/// 这里处理与“世界/全局”相关的钩子，并把事件分发给各个 <see cref="BaseEnhance"/>。
	/// </para>
	/// </summary>
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
        /// <summary>
        /// 记录尸块伤害（火焰猫燐能力相关）
        /// </summary>
        public static int[] GoreDamage = new int[Main.maxGore];
        private static void ProcessDemonismAction(Action<BaseEnhance> action)
        {
            // 系统级钩子默认对“已注册的全部增强”分发（不基于某个玩家启用列表）。
            foreach (BaseEnhance enhance in EnhanceRegistry.AllEnhancements)
            {
                action(enhance);
            }
        }
        public override void ModifyLightingBrightness(ref float scale)
        {
            // 把 ref 参数先拷贝到局部变量，避免闭包捕获 ref 的限制。
            float scale2 = scale;
            ProcessDemonismAction((enhance) => enhance.SystemModifyLightingBrightness(ref scale2));
            scale = scale2;
        }
        public override void PostSetupContent()
        {
            // 预计算城镇 NPC 总量（后续某些增强/BUFF 会用到）。
            TownNPCMax = ContentSamples.NpcsByNetId.Values.Count(npc => npc.townNPC);

            ProcessDemonismAction((enhance) => enhance.SystemPostSetupContent());
        }
        public override void PostAddRecipes()
        {
            // 配方添加完成后分发给增强，用于在此阶段补充/修改配方。
            ProcessDemonismAction((enhance) => enhance.SystemPostAddRecipes());
        }
        public override void PreUpdateGores()
        {
            // 清理“已经失活的 gore”对应的伤害缓存，避免误伤/残留。
            for (int i = 0; i < Main.maxGore; i++)
                if (!Main.gore[i].active)
                    GoreDamage[i] = 0;

            ProcessDemonismAction((enhance) => enhance.SystemPreUpdateGores());
        }
        public override void PostUpdateNPCs()
        {
            TownNPCCount = 0;

            // 特判：血月/满月夜晚时视为城镇 NPC 全部存在（某些效果按此计算）。
            if (!Main.dayTime && (Main.bloodMoon || Main.GetMoonPhase() == Terraria.Enums.MoonPhase.Full))
                TownNPCCount = TownNPCMax;
            else foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.townNPC)
                        TownNPCCount++;
                }

            // 客户端只做计数，不做需要服务器权威的逻辑。
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            ProcessDemonismAction((enhance) => enhance.SystemPostUpdateNPCs());
        }
        public override void PostUpdateEverything()
        {
            // 一帧内所有 Update 完成后的总收尾钩子。
            ProcessDemonismAction((enhance) => enhance.SystemPostUpdateEverything());
        }
    }
}
