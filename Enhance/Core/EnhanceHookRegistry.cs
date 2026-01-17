using System;
using System.Collections.Generic;
using System.Reflection;

namespace TouhouPetsEx.Enhance.Core
{
	/// <summary>
	/// 增强钩子注册表：在加载期扫描每个 <see cref="BaseEnhance"/> 是否覆写了某些钩子，
	/// 并把“真正实现了钩子”的实例放进对应列表，避免运行期对所有增强做空调用。
	/// </summary>
	public static class EnhanceHookRegistry
	{
		/// <summary>
		/// 防止重复登记同一种增强类型（每个派生类型只登记一次）。
		/// </summary>
		private static readonly HashSet<Type> RegisteredTypes = [];

		/// <summary>实现了 <see cref="BaseEnhance.TileDrawEffects"/> 的增强列表。</summary>
		public static readonly List<BaseEnhance> TileDrawEffects = [];
		/// <summary>实现了 <see cref="BaseEnhance.TileRandomUpdate"/> 的增强列表。</summary>
		public static readonly List<BaseEnhance> TileRandomUpdate = [];

        /// <summary>实现了 <see cref="BaseEnhance.NPCOnSpawn"/> 的增强列表。</summary>
        public static readonly List<BaseEnhance> NPCOnSpawn = [];
        /// <summary>实现了 <see cref="BaseEnhance.NPCPreAI"/> 的增强列表。</summary>
        public static readonly List<BaseEnhance> NPCPreAI = [];
		/// <summary>实现了 <see cref="BaseEnhance.NPCAI"/> 的增强列表。</summary>
		public static readonly List<BaseEnhance> NPCAI = [];
		/// <summary>实现了 <see cref="BaseEnhance.NPCCanHitNPC"/> 的增强列表。</summary>
		public static readonly List<BaseEnhance> NPCCanHitNPC = [];

        /// <summary>实现了 <see cref="BaseEnhance.ProjectileOnSpawn"/> 的增强列表。</summary>
        public static readonly List<BaseEnhance> ProjectileOnSpawn = [];

        /// <summary>
        /// 注册一个增强实例：识别其覆写的钩子并加入对应列表。
        /// </summary>
        /// <param name="enhance">增强实例。</param>
        public static void Register(BaseEnhance enhance)
		{
			// 传入为空时直接忽略，避免 NRE。
			if (enhance == null)
				return;

			Type type = enhance.GetType();
			// 一个增强类型只登记一次：同类型多实例会导致重复调用。
			if (!RegisteredTypes.Add(type))
				return;

			// 按需登记：只有覆写了该方法才加入列表，减少运行期开销。
			if (IsOverridden(type, nameof(BaseEnhance.TileDrawEffects)))
				TileDrawEffects.Add(enhance);
			if (IsOverridden(type, nameof(BaseEnhance.TileRandomUpdate)))
				TileRandomUpdate.Add(enhance);

            if (IsOverridden(type, nameof(BaseEnhance.NPCOnSpawn)))
                NPCOnSpawn.Add(enhance);
            if (IsOverridden(type, nameof(BaseEnhance.NPCPreAI)))
				NPCPreAI.Add(enhance);
			if (IsOverridden(type, nameof(BaseEnhance.NPCAI)))
				NPCAI.Add(enhance);
			if (IsOverridden(type, nameof(BaseEnhance.NPCCanHitNPC)))
				NPCCanHitNPC.Add(enhance);

            if (IsOverridden(type, nameof(BaseEnhance.ProjectileOnSpawn)))
                ProjectileOnSpawn.Add(enhance);
        }

		/// <summary>
		/// 清空所有注册信息（通常在 Mod 卸载时调用）。
		/// </summary>
		public static void Clear()
		{
			// 清空“已登记类型”后，下次加载可重新注册。
			RegisteredTypes.Clear();
			// 清空每个钩子列表，避免卸载后残留引用导致潜在泄漏/重复调用。
			TileDrawEffects.Clear();
			TileRandomUpdate.Clear();
			NPCOnSpawn.Clear();
			NPCPreAI.Clear();
			NPCAI.Clear();
			NPCCanHitNPC.Clear();
            ProjectileOnSpawn.Clear();

        }

		/// <summary>
		/// 判断某个增强类型是否覆写了指定方法。
		/// </summary>
		/// <param name="type">增强类型。</param>
		/// <param name="methodName">方法名（来自 <see cref="nameof"/>）。</param>
		/// <returns>如果该方法的声明类型不是 <see cref="BaseEnhance"/>，则认为已覆写。</returns>
		private static bool IsOverridden(Type type, string methodName)
		{
			// 只查 public instance（BaseEnhance 的钩子方法都是 public virtual）。
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			return method != null && method.DeclaringType != typeof(BaseEnhance);
		}
	}
}

