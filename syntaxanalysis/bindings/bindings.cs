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
        public Boundunaryexpression(Boundunaryoperatorkind operatorkind, Boundexpression operand)
        {
            Operatorkind=operatorkind;
            Operand=operand;
        }

        public Boundunaryoperatorkind Operatorkind { get; }
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
    internal sealed class BoundBinaryexpression : Boundexpression
    {
        

        public BoundBinaryexpression(Boundexpression left, BoundBinaryoperatorkind boundoperatorkind , Boundexpression right)
        {
            Left = left;
            Boundoperatorkind = boundoperatorkind;
            Right = right;
        }

        public Boundexpression Left { get; }
       // public Boundnodekind Operatorkind { get; }
        public Boundexpression Right { get; }

        public override Type Type => Boundoperatorkind == BoundBinaryoperatorkind.Equals ? typeof(bool) : Left.Type;

        public BoundBinaryoperatorkind Boundoperatorkind { get; }

        internal override Boundnodekind Boundnodekind => Boundnodekind.Binaryexpression;
    }



}
