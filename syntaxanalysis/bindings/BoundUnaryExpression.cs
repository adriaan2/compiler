using System;

namespace bindings
{
    internal sealed class Boundunaryexpression : Boundexpression
    {
        public Boundunaryexpression(Boundunaryoperatorkind operatorkind, Boundexpression operand)
        {
            Operatorkind = operatorkind;
            Operand = operand;
        }

        public Boundunaryoperatorkind Operatorkind { get; }
        public Boundexpression Operand { get; }

        public override Type Type => Operand.Type;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Unaryexpression;
    }
}
