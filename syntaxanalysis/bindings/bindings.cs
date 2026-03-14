using System;
 namespace bindings
{

    
    internal abstract class Boundnode
    {
        internal abstract Boundnodekind Boundnodekind{get;}
    }
    internal abstract class Boundexpression : Boundnode
    {
        public abstract Type Type{get;}
    }
    internal sealed class Boundunaryexpression:Boundexpression
    {
        public Boundunaryexpression(Boundnodekind operatorkind, Boundexpression operand)
        {
            Operatorkind=operatorkind;
            Operand=operand;
        }

        public Boundnodekind Operatorkind { get; }
        public Boundexpression Operand { get; }

        public override Type Type => Operand.Type;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Unaryexpression;

    }
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
    internal sealed class BoundBinaryexpression : Boundexpression
    {
        public BoundBinaryexpression(Boundexpression left, Boundnodekind operatorkind, Boundexpression right)
        {
            Left = left;
            Operatorkind = operatorkind;
            Right = right;
        }

        public Boundexpression Left { get; }
        public Boundnodekind Operatorkind { get; }
        public Boundexpression Right { get; }

        public override Type Type => Left.Type;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Binaryexpression;
    }



}
