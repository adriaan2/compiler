using System;

namespace bindings
{
    internal sealed class BoundAssignmentexpression : Boundexpression
    {
        public BoundAssignmentexpression(VariableSymbol variable, Boundexpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public VariableSymbol Variable { get; }
        public Boundexpression Expression { get; }

        public override Type Type => Expression.Type;

        internal override Boundnodekind Boundnodekind => Boundnodekind.Assignmentexpression;
    }
}
