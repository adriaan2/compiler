using System;

namespace bindings
{
    internal sealed class BoundVariabledeclarationexpression : Boundexpression
    {
        public BoundVariabledeclarationexpression(VariableSymbol variable, Boundexpression initializer)
        {
            Variable = variable;
            Initializer = initializer;
        }

        public VariableSymbol Variable { get; }
        public Boundexpression Initializer { get; }

        public override Type Type => Variable.Type;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Variabledeclarationexpression;
    }
}
