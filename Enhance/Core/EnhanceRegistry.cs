using System;
using System.Collections.Generic;

namespace TouhouPetsEx.Enhance.Core
{
    /// <summary>
    /// 增强注册表：
    /// <list type="bullet">
    /// <item><description>身份层：<see cref="EnhancementId"/> → <see cref="BaseEnhance"/></description></item>
    /// <item><description>来源层：<c>itemType(int)</c> → <see cref="EnhancementId"/></description></item>
    /// </list>
    /// 用于把“增强身份”从 <c>item.type</c> 解耦，支持“一增强多物品”等扩展。
    /// </summary>
    public static class EnhanceRegistry
    {
        private static readonly Dictionary<EnhancementId, BaseEnhance> EnhanceById = [];
        private static readonly Dictionary<int, EnhancementId> EnhanceIdByItemType = [];
        private static readonly Dictionary<EnhancementId, HashSet<int>> ItemTypesByEnhanceId = [];

        /// <summary>
        /// 当前已注册的全部增强实例（按增强身份去重）。
        /// </summary>
        public static IEnumerable<BaseEnhance> AllEnhancements => EnhanceById.Values;

        /// <summary>
        /// 注册增强实例（仅注册身份层：<see cref="EnhancementId"/> → <see cref="BaseEnhance"/>）。
        /// </summary>
        /// <param name="enhance">增强实例。</param>
        public static void RegisterEnhancement(BaseEnhance enhance)
        {
            if (enhance == null)
                return;

            // 身份层登记：同一增强 ID 只保留一个实例，避免后续分发重复调用。
            EnhanceById.TryAdd(enhance.EnhanceId, enhance);
        }

        /// <summary>
        /// 绑定物品类型到增强身份（注册身份层并建立来源层映射）。
        /// </summary>
        /// <param name="enhance">增强实例。</param>
        /// <param name="itemType">物品 type（通常为 <c>ModContent.ItemType&lt;T&gt;()</c>）。</param>
        public static void BindItemType(BaseEnhance enhance, int itemType)
        {
            if (enhance == null)
                return;

            // 先登记增强实例到“身份层”，再建立来源映射。
            RegisterEnhancement(enhance);

            EnhancementId id = enhance.EnhanceId;
            // 来源层映射：itemType -> enhanceId（支持多个 itemType 指向同一个增强）。
            EnhanceIdByItemType[itemType] = id;

            // 反向查询：enhanceId -> {itemTypes...}，用于“一增强多物品”的统计/调试/功能扩展。
            if (!ItemTypesByEnhanceId.TryGetValue(id, out var itemTypes))
            {
                itemTypes = [];
                ItemTypesByEnhanceId.Add(id, itemTypes);
            }

            itemTypes.Add(itemType);
        }

        /// <summary>
        /// 从物品 type 查找对应的增强身份。
        /// </summary>
        /// <param name="itemType">物品 type。</param>
        /// <param name="enhanceId">输出增强身份。</param>
        /// <returns>若存在映射则为 true。</returns>
        public static bool TryGetEnhanceId(int itemType, out EnhancementId enhanceId)
        {
            // 来源层查询：由物品 type 获取增强身份。
            return EnhanceIdByItemType.TryGetValue(itemType, out enhanceId);
        }

        /// <summary>
        /// 从增强身份查找增强实例。
        /// </summary>
        /// <param name="enhanceId">增强身份。</param>
        /// <param name="enhancement">输出增强实例。</param>
        /// <returns>若已注册则为 true。</returns>
        public static bool TryGetEnhancement(EnhancementId enhanceId, out BaseEnhance enhancement)
        {
            // 身份层查询：由增强身份获取增强实例。
            return EnhanceById.TryGetValue(enhanceId, out enhancement);
        }

        /// <summary>
        /// 从增强身份获取增强实例（未注册会抛异常）。
        /// </summary>
        /// <param name="enhanceId">增强身份。</param>
        /// <returns>增强实例。</returns>
        public static BaseEnhance GetEnhancement(EnhancementId enhanceId)
        {
            // 强制获取：若缺失则抛异常，便于尽早暴露注册/映射问题。
            return EnhanceById[enhanceId];
        }

        /// <summary>
        /// 从物品 type 获取对应的增强实例（等价于先查 <see cref="TryGetEnhanceId"/> 再查 <see cref="TryGetEnhancement"/>）。
        /// </summary>
        /// <param name="itemType">物品 type。</param>
        /// <param name="enhancement">输出增强实例。</param>
        /// <returns>若存在映射且已注册则为 true。</returns>
        public static bool TryGetEnhancementByItemType(int itemType, out BaseEnhance enhancement)
        {
            enhancement = null;
            // 组合查询：itemType -> enhanceId -> enhancement。
            if (!EnhanceIdByItemType.TryGetValue(itemType, out EnhancementId enhanceId))
                return false;
            return EnhanceById.TryGetValue(enhanceId, out enhancement);
        }

        /// <summary>
        /// 获取某增强身份目前绑定的所有物品 type（用于“一增强多物品”等场景）。
        /// </summary>
        /// <param name="enhanceId">增强身份。</param>
        /// <returns>绑定的物品 type 集合；若不存在则返回空集合。</returns>
        public static IReadOnlyCollection<int> GetBoundItemTypes(EnhancementId enhanceId)
        {
            // 若没有绑定记录则返回空集合，避免返回 null。
            return ItemTypesByEnhanceId.TryGetValue(enhanceId, out var set) ? set : Array.Empty<int>();
        }

        /// <summary>
        /// 清空注册表（通常在 Mod 卸载时调用）。
        /// </summary>
        public static void Clear()
        {
            // 卸载时清理静态引用，避免重复加载时出现脏数据。
            EnhanceById.Clear();
            EnhanceIdByItemType.Clear();
            ItemTypesByEnhanceId.Clear();
        }
    }
}
