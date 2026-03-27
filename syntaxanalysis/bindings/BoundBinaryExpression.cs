using System;

namespace bindings
{
    internal sealed class BoundBinaryexpression : Boundexpression
    {
        public BoundBinaryexpression(Boundexpression left, BoundBinaryoperatorkind boundoperatorkind, Boundexpression right)
        {
            Left = left;
            Boundoperatorkind = boundoperatorkind;
            Right = right;
        }

        public Boundexpression Left { get; }
        public Boundexpression Right { get; }
        public BoundBinaryoperatorkind Boundoperatorkind { get; }

        public override Type Type => Boundoperatorkind == BoundBinaryoperatorkind.Equals ? typeof(bool) : Left.Type;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Binaryexpression;
    }
}
