using System;
using System.Collections.Generic;
using System.Reflection;

namespace TouhouPetsEx.Enhance.Core
{
	public static class EnhanceHookRegistry
	{
		private static readonly HashSet<Type> RegisteredTypes = new();

		public static readonly List<BaseEnhance> TileDrawEffects = new();
		public static readonly List<BaseEnhance> TileRandomUpdate = new();

		public static readonly List<BaseEnhance> NPCPreAI = new();
		public static readonly List<BaseEnhance> NPCAI = new();
		public static readonly List<BaseEnhance> NPCCanHitNPC = new();

		public static void Register(BaseEnhance enhance)
		{
			if (enhance == null)
				return;

			Type type = enhance.GetType();
			if (!RegisteredTypes.Add(type))
				return;

			if (IsOverridden(type, nameof(BaseEnhance.TileDrawEffects)))
				TileDrawEffects.Add(enhance);
			if (IsOverridden(type, nameof(BaseEnhance.TileRandomUpdate)))
				TileRandomUpdate.Add(enhance);

			if (IsOverridden(type, nameof(BaseEnhance.NPCPreAI)))
				NPCPreAI.Add(enhance);
			if (IsOverridden(type, nameof(BaseEnhance.NPCAI)))
				NPCAI.Add(enhance);
			if (IsOverridden(type, nameof(BaseEnhance.NPCCanHitNPC)))
				NPCCanHitNPC.Add(enhance);
		}

		public static void Clear()
		{
			RegisteredTypes.Clear();
			TileDrawEffects.Clear();
			TileRandomUpdate.Clear();
			NPCPreAI.Clear();
			NPCAI.Clear();
			NPCCanHitNPC.Clear();
		}

		private static bool IsOverridden(Type type, string methodName)
		{
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			return method != null && method.DeclaringType != typeof(BaseEnhance);
		}
	}
}

