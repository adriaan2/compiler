using System;

namespace bindings
{
    internal sealed class BoundVariableexpression : Boundexpression
    {
        public BoundVariableexpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public VariableSymbol Variable { get; }

        public override Type Type => Variable.Type;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Variableexpression;
    }
}
