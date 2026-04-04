using System;

namespace bindings
{
    internal sealed class BoundBinaryoperator
    {
        private BoundBinaryoperator(Boundexpression left, BoundBinaryoperatorkind kind, Boundexpression right)
        {
            Left = left;
            Kind = kind;
            Right = right;
        }

        private BoundBinaryoperator(Boundexpression left, BoundBinaryoperatorkind kind, Boundexpression right, Type operandType)
        {
            Left = left;
            Kind = kind;
            Right = right;
            OperandType = operandType;
        }

        public Boundexpression Left { get; }
        public BoundBinaryoperatorkind Kind { get; }
        public Boundexpression Right { get; }
        public Type OperandType { get; }

        private static BoundBinaryoperator bind()
        {
            throw new NotImplementedException();
        }
    }
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

        public override Type Type =>
            Boundoperatorkind == BoundBinaryoperatorkind.Equals
            || Boundoperatorkind == BoundBinaryoperatorkind.LogicalAnd
            || Boundoperatorkind == BoundBinaryoperatorkind.LogicalOr
                ? typeof(bool)
                : Left.Type;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Binaryexpression;
    }
}
