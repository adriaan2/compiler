using bindings;

internal sealed class BoundEvaluator
{
    private readonly Boundexpression _root;
    private  readonly Dictionary<VariableSymbol, object> _variables;

    public BoundEvaluator(Boundexpression root, Dictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;
    }

    public object Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private  object EvaluateExpression(Boundexpression node)
    {
        if (node is BoundNumberexpression number)
            return number.Value;

        if (node is BoundBooleanexpression boolean)
            return boolean.Value;

        if (node is Boundunaryexpression unary)
        {
            var operand = EvaluateExpression(unary.Operand);

            switch (unary.Operatorkind)
            {
                case Boundunaryoperatorkind.Identity:
                    return (int)operand;
                case Boundunaryoperatorkind.Negation:
                    return -(int)operand;
                case Boundunaryoperatorkind.LogicalNegation:
                    return !(bool)operand;
                default:
                    throw new Exception($"Unexpected unary operator {unary.Operatorkind}");
            }
        }

        if (node is BoundBinaryexpression binary)
        {
            var left = EvaluateExpression(binary.Left);
            var right = EvaluateExpression(binary.Right);

            switch (binary.Boundoperatorkind)
            {
                case BoundBinaryoperatorkind.addition:
                    return (int)left + (int)right;
                case BoundBinaryoperatorkind.subtraction:
                    return (int)left - (int)right;
                case BoundBinaryoperatorkind.times:
                    return (int)left * (int)right;
                case BoundBinaryoperatorkind.division:
                    return (int)left / (int)right;
                case BoundBinaryoperatorkind.Equals:
                    return Equals(left, right);
                case BoundBinaryoperatorkind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryoperatorkind.LogicalOr:
                    return (bool)left || (bool)right;
                default:
                    throw new Exception($"Unexpected binary operator {binary.Boundoperatorkind}");
            }
        }

        if (node is BoundVariableexpression variable)
        {
            if (_variables.TryGetValue(variable.Variable, out var value))
                return value;

            throw new Exception($"Variable '{variable.Variable.Name}' was not assigned.");
        }

        if (node is BoundAssignmentexpression assignment)
        {
            var value = EvaluateExpression(assignment.Expression);
            _variables[assignment.Variable] = value;
            return value;
        }

        if (node is BoundVariabledeclarationexpression declaration)
        {
            var value = EvaluateExpression(declaration.Initializer);
            _variables[declaration.Variable] = value;
            return value;
        }

        throw new Exception($"Unexpected bound node {node.Boundnodekind}");
    }
}
