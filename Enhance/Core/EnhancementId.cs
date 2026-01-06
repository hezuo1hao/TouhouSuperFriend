using System;

namespace TouhouPetsEx.Enhance.Core
{
    /// <summary>
    /// 增强的强类型身份标识。
    /// <para>
    /// 设计目的：替代裸 <see cref="string"/>，避免“记字符串/写错字符串但编译通过”的问题，并作为增强系统的稳定 Key。
    /// </para>
    /// </summary>
    public readonly record struct EnhancementId
    {
        /// <summary>
        /// 序列化载体（存档/联机包使用）。
        /// </summary>
        public string Value { get; }

        private EnhancementId(string value)
        {
            // 统一保证 ID 非空，避免把无效 Key 写进存档或注册表。
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("EnhancementId cannot be null/empty.", nameof(value));

            Value = value;
        }

        /// <summary>
        /// 从字符串还原增强 ID（仅供本程序集内部用于存档/联机反序列化）。
        /// </summary>
        /// <param name="value">序列化载体。</param>
        internal static EnhancementId From(string value) => new(value);

        /// <summary>
        /// 从类型生成增强 ID（默认策略：使用 <see cref="Type.FullName"/>）。
        /// </summary>
        /// <param name="type">增强类型。</param>
        /// <returns>增强 ID。</returns>
        public static EnhancementId FromType(Type type)
        {
            // 类型为 null 时直接报错，避免返回“看似有效”的默认值。
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return new EnhancementId(type.FullName ?? type.Name);
        }

        /// <summary>
        /// 获取某类型对应的增强 ID（常用入口，避免手写字符串）。
        /// </summary>
        /// <typeparam name="T">增强类型。</typeparam>
        /// <returns>增强 ID。</returns>
        public static EnhancementId Of<T>()
        {
            // 直接用 typeof(T) 生成，保持调用端简单、可自动补全。
            return FromType(typeof(T));
        }

        /// <summary>
        /// 返回序列化载体字符串。
        /// </summary>
        public override string ToString() => Value;
    }
}
