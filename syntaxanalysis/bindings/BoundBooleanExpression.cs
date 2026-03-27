using System;

namespace bindings
{
    internal sealed class BoundBooleanexpression : Boundexpression
    {
        public BoundBooleanexpression(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        public override Type Type => typeof(bool);

        internal override Boundnodekind Boundnodekind => Boundnodekind.Booleanexpression;
    }
}
