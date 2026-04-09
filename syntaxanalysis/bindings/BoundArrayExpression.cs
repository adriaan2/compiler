using System;

namespace bindings
{
    internal sealed class BoundArrayexpression : Boundexpression
    {
        public BoundArrayexpression(Type elementType, IReadOnlyList<Boundexpression> elements)
        {
            ElementType = elementType;
            Elements = elements;
        }

        public Type ElementType { get; }
        public IReadOnlyList<Boundexpression> Elements { get; }

        public override Type Type => typeof(ArrayValue);

        internal override Boundnodekind Boundnodekind => Boundnodekind.Arrayexpression;
    }
}
