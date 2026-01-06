using System;
using System.Collections.Generic;

namespace TouhouPetsEx.Enhance.Core
{
    public static class EnhanceRegistry
    {
        private static readonly Dictionary<EnhancementId, BaseEnhance> EnhanceById = new();
        private static readonly Dictionary<int, EnhancementId> EnhanceIdByItemType = new();
        private static readonly Dictionary<EnhancementId, HashSet<int>> ItemTypesByEnhanceId = new();

        public static IEnumerable<BaseEnhance> AllEnhancements => EnhanceById.Values;

        public static void RegisterEnhancement(BaseEnhance enhance)
        {
            if (enhance == null)
                return;

            EnhanceById.TryAdd(enhance.EnhanceId, enhance);
        }

        public static void BindItemType(BaseEnhance enhance, int itemType)
        {
            if (enhance == null)
                return;

            RegisterEnhancement(enhance);

            EnhancementId id = enhance.EnhanceId;
            EnhanceIdByItemType[itemType] = id;

            if (!ItemTypesByEnhanceId.TryGetValue(id, out var itemTypes))
            {
                itemTypes = new HashSet<int>();
                ItemTypesByEnhanceId.Add(id, itemTypes);
            }

            itemTypes.Add(itemType);
        }

        public static bool TryGetEnhanceId(int itemType, out EnhancementId enhanceId)
        {
            return EnhanceIdByItemType.TryGetValue(itemType, out enhanceId);
        }

        public static bool TryGetEnhancement(EnhancementId enhanceId, out BaseEnhance enhancement)
        {
            return EnhanceById.TryGetValue(enhanceId, out enhancement);
        }

        public static BaseEnhance GetEnhancement(EnhancementId enhanceId)
        {
            return EnhanceById[enhanceId];
        }

        public static bool TryGetEnhancementByItemType(int itemType, out BaseEnhance enhancement)
        {
            enhancement = null;
            if (!EnhanceIdByItemType.TryGetValue(itemType, out EnhancementId enhanceId))
                return false;
            return EnhanceById.TryGetValue(enhanceId, out enhancement);
        }

        public static IReadOnlyCollection<int> GetBoundItemTypes(EnhancementId enhanceId)
        {
            return ItemTypesByEnhanceId.TryGetValue(enhanceId, out var set) ? set : Array.Empty<int>();
        }

        public static void Clear()
        {
            EnhanceById.Clear();
            EnhanceIdByItemType.Clear();
            ItemTypesByEnhanceId.Clear();
        }
    }
}
