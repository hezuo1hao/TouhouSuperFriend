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
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Buffs;

namespace TouhouPetsEx.Enhance.Core
{
	/// <summary>
	/// 增强相关的全局物品钩子分发。
	/// <para>
	/// 负责把 tML 的 Item 相关回调分发到当前启用的增强（<see cref="EnhancePlayers.ActiveEnhance"/> / <see cref="EnhancePlayers.ActivePassiveEnhance"/>）。
	/// </para>
	/// </summary>
	public class GEnhanceItems : GlobalItem
    {
        #region 防止闭包的私有字段们
        int GrabRange_grabRange;
        float HorizontalWingSpeeds_speed;
        float HorizontalWingSpeeds_acceleration;

        #endregion
        private static void ProcessDemonismAction(Action<BaseEnhance> action)
        {
            // 全局分发：对所有已注册增强执行一次（用于与具体物品无关的全局事件）。
            foreach (BaseEnhance enhance in EnhanceRegistry.AllEnhancements)
            {
                action(enhance);
            }
        }
        private static void ProcessDemonismAction(Item item, Action<BaseEnhance> action)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance))
            {
                // 单物品分发：只对“该物品绑定的增强”执行。
                action(enhance);
            }
        }
        private static void ProcessDemonismAction(Player player, Action<BaseEnhance> action)
        {
            if (!player.HasTouhouPetsBuff())
                return;

            // 玩家分发：只对玩家当前启用的增强执行。
            foreach (EnhancementId enhanceId in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
            {
                if (EnhanceRegistry.TryGetEnhancement(enhanceId, out var enhancement))
                    action(enhancement);
            }
        }
        private static bool? ProcessDemonismAction(Item item, Func<BaseEnhance, bool?> action)
        {
            bool? @return = null;
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance))
            {
                // 对“该物品绑定的增强”执行一次，并返回其结果（可为 null）。
                @return = action(enhance);
            }
            return @return;
        }
        /// <param name="priority">填写需要优先返回的bool结果，如：执行三次，俩false一true，需求true，则返回true结果
        /// <br>特别的，如果填null则会返回最后一个非null的结果</br>
        /// </param>
        private static bool? ProcessDemonismAction(Player player, bool? priority, Func<BaseEnhance, bool?> action)
        {
            if (!player.HasTouhouPetsBuff())
                return null;

            if (priority == null)
            {
                bool? @return = null;
                foreach (EnhancementId enhanceId in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
                {
                    if (!EnhanceRegistry.TryGetEnhancement(enhanceId, out var enhancement))
                        continue;

                    // priority=null：返回“最后一个非 null 的结果”，用于链式覆盖。
                    bool? a = action(enhancement);
                    if (a != null) @return = a;
                }
                return @return;
            }
            else
            {
                bool? @return = null;
                foreach (EnhancementId enhanceId in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
                {
                    if (!EnhanceRegistry.TryGetEnhancement(enhanceId, out var enhancement))
                        continue;

                    // priority!=null：遇到目标值直接短路返回（例如 true/false 的“强制优先”）。
                    bool? a = action(enhancement);
                    if (a == priority) return a;
                    else if (a != null) @return = a;
                }
                return @return;
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
                foreach (BaseEnhance enhance in EnhanceRegistry.AllEnhancements)
                {
                    // priority=null：返回“最后一个非 null 的结果”。
                    bool? a = action(enhance);
                    if (a != null) @return = a;
                }
                return @return;
            }
            else
            {
                bool? @return = null;
                foreach (BaseEnhance enhance in EnhanceRegistry.AllEnhancements)
                {
                    // priority!=null：优先返回目标结果。
                    bool? a = action(enhance);
                    if (a == priority) return a;
                    else if (a != null) @return = a;
                }
                return @return;
            }
        }
        public override bool InstancePerEntity => true;
        public override void SetDefaults(Item entity)
        {
            // SetDefaults：对“该物品绑定的增强”分发一次。
            ProcessDemonismAction(entity, (enhance) => enhance.ItemSD(entity));
        }

        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            if (player.MBP().Throw)
                grabRange += 1600;

            GrabRange_grabRange = grabRange;
            ProcessDemonismAction(player, (enhance) => enhance.ItemGrabRange(item, player, ref GrabRange_grabRange));
            grabRange = GrabRange_grabRange;
        }
        public override bool GrabStyle(Item item, Player player)
        {
            if (player.MBP().Throw)
            {
                player.PullItem_Pickup(item, 24f, 5);
                return true;
            }

            bool? reesult = ProcessDemonismAction(player, true, (enhance) => enhance.ItemGrabStyle(item, player));

            return reesult ?? base.GrabStyle(item, player);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets"
                && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance)
                && enhance.Passive
                && !player.EnableEnhance(item.type)
                && EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
            {
                if (!player.MP().ActivePassiveEnhance.Contains(enhanceId))
                    player.MP().ActivePassiveEnhance.Add(enhanceId);
            }

            ProcessDemonismAction((enhance) => enhance.ItemHoldItem(item, player));
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets"
                && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance)
                && enhance.Passive
                && !player.EnableEnhance(item.type)
                && EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
            {
                if (!player.MP().ActivePassiveEnhance.Contains(enhanceId))
                    player.MP().ActivePassiveEnhance.Add(enhanceId);
            }

            ProcessDemonismAction((enhance) => enhance.ItemUpdateInventory(item, player));
        }
        public override bool? UseItem(Item item, Player player)
        {
            // UseItem：聚合多个增强的可选 bool? 结果，优先返回 false（阻止默认行为）。
            return ProcessDemonismAction(player, false, (enhance) => enhance.ItemUseItem(item, player));
        }
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (ItemID.Sets.IsFood[item.type])
            {
                var mp = Main.LocalPlayer.GetModPlayer<EnhanceBuffPlayers>();
                if (mp.Glutton && mp.Patience && mp.Throw)
                    ModContent.GetInstance<TouhouMystiasIzakaya>().Condition.Complete();
            }

            ProcessDemonismAction((enhance) => enhance.ItemOnCreated(item, context));
        }
        public override void SetStaticDefaults()
        {
            List<Type> subclasses = [];

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] allTypes = assembly.GetTypes();

            foreach (Type type in allTypes)
            {
                if (type.IsClass && !type.IsAbstract && typeof(BaseEnhance).IsAssignableFrom(type))
                {
                    subclasses.Add(type);
                }
            }

            foreach (Type types in subclasses)
            {
                object enhance = Activator.CreateInstance(types);
                BaseEnhance thisEnhance = enhance as BaseEnhance;
                // 身份层登记：允许后续用 EnhancementId 找到增强实例。
                EnhanceRegistry.RegisterEnhancement(thisEnhance);
                // 由增强自行声明绑定的物品 type，并建立 itemType -> EnhancementId 映射。
                thisEnhance.ItemSSD();
                // 性能优化：登记覆写过的钩子，避免运行期空调用。
                EnhanceHookRegistry.Register(thisEnhance);
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enh))
            {
                int index = tooltips.GetTooltipsLastIndex();
                int experimentalTooltip = 0;
                tooltips.Insert(index + 1, new TooltipLine(Mod, "EnhanceTooltip", GetText("Common") + "\n" + (enh.Passive ? GetText("Passive") + "\n" : "") + enh.Text));

                for (int i = 0; i < enh.Experimental.Length; i++)
                {
                    if (enh.Experimental[i])
                    {
                        if (experimentalTooltip == 0)
                        {
                            tooltips.Insert(index + 2, new TooltipLine(Mod, "EnhanceTooltip_Experimental", GetText("Experimental")));
                            experimentalTooltip++;
                        }

                        tooltips.Insert(index + 2 + experimentalTooltip, new TooltipLine(Mod, "EnhanceTooltip_Experimental", enh.ExperimentalText[i]));
                        experimentalTooltip++;
                    }
                }
            }

            ProcessDemonismAction((enhance) => enhance.ItemModifyTooltips(item, tooltips));
        }
        public override bool AltFunctionUse(Item item, Player player)
        {
            // 控制是否允许右键（由增强决定）。
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enh))
                return enh.EnableRightClick;

            return false;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            bool def = true;
            // 让增强可以拦截/修改 CanUseItem，并允许修改 def。
            bool? reesult = ProcessDemonismAction(player, false, (enhance) => enhance.ItemCanUseItem(item, player, ref def));

            if (reesult.HasValue)
                return reesult.Value;

            if (def && item.ModItem?.Mod.Name == "TouhouPets" && player.altFunctionUse == 2 && player.HasTouhouPetsBuff()
                && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enh) && enh.EnableRightClick)
            {
                // 右键：切换“该物品绑定的增强”是否启用。
                if (!EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                    return true;

                if (player.MP().ActiveEnhance.Remove(enhanceId))
                {
                    CombatText.NewText(player.getRect(), Color.Cyan, GetText("Disable"));
                }
                else
                {
                    if (player.MP().ActiveEnhance.Count >= player.MP().ActiveEnhanceCount)
                    {
                        player.MP().ActiveEnhance.RemoveAt(0);
                        player.MP().ActiveEnhance.Add(enhanceId);
                    }
                    else
                        player.MP().ActiveEnhance.Add(enhanceId);

                    CombatText.NewText(player.getRect(), Color.Cyan, GetText("Enable"));
                }

                EnhancePlayers.AwardPlayerSync(Mod, -1, player.whoAmI);

                // 返回 false：阻止物品原本的使用逻辑（右键用于开关增强）。
                return false;
            }

            return true;
        }
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            HorizontalWingSpeeds_speed = speed;
            HorizontalWingSpeeds_acceleration = acceleration;
            ProcessDemonismAction(player, (enhance) => enhance.ItemHorizontalWingSpeeds(item, player, ref HorizontalWingSpeeds_speed, ref HorizontalWingSpeeds_acceleration));
            speed = HorizontalWingSpeeds_speed;
            acceleration = HorizontalWingSpeeds_acceleration;
        }
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            bool? reesult = ProcessDemonismAction(Main.LocalPlayer, false, (enhance) => enhance.ItemPreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale));

            return reesult ?? base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            ProcessDemonismAction((enhance) => enhance.ItemPostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale));
        }
    }
}
