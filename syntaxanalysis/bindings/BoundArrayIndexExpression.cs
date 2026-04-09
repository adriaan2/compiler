using System;

namespace bindings
{
    internal sealed class BoundArrayindexexpression : Boundexpression
    {
        public BoundArrayindexexpression(Boundexpression array, Boundexpression index, Type elementType)
        {
            Array = array;
            Index = index;
            ElementType = elementType;
        }

        public Boundexpression Array { get; }
        public Boundexpression Index { get; }
        public Type ElementType { get; }

        public override Type Type => ElementType;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Arrayindexexpression;
    }
}
