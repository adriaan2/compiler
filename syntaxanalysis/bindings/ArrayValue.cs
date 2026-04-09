using System;

namespace bindings
{
    internal sealed class ArrayValue
    {
        public ArrayValue(Type elementType, object[] elements)
        {
            ElementType = elementType;
            Elements = elements;
        }

        public Type ElementType { get; }
        public object[] Elements { get; }
        public int Length => Elements.Length;

        public override string ToString()
        {
            return $"[{string.Join(", ", Elements)}]";
        }
    }
}
