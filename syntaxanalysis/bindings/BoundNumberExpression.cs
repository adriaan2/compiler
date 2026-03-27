using System;

namespace bindings
{
    internal sealed class BoundNumberexpression : Boundexpression
    {
        public BoundNumberexpression(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override Type Type => typeof(int);

        internal override Boundnodekind Boundnodekind => Boundnodekind.Numberexpression;
    }
}
