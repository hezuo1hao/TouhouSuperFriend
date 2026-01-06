using System;

namespace TouhouPetsEx.Enhance.Core
{
    public readonly record struct EnhancementId
    {
        public string Value { get; }

        private EnhancementId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("EnhancementId cannot be null/empty.", nameof(value));

            Value = value;
        }

        internal static EnhancementId From(string value) => new(value);

        public static EnhancementId FromType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return new EnhancementId(type.FullName ?? type.Name);
        }

        public static EnhancementId Of<T>()
        {
            return FromType(typeof(T));
        }

        public override string ToString() => Value;
    }
}
