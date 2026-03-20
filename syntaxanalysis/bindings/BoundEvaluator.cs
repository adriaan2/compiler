using bindings;

internal sealed class BoundEvaluator
{
    private readonly Boundexpression _root;

    public BoundEvaluator(Boundexpression root)
    {
        _root = root;
    }

    public object Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private static object EvaluateExpression(Boundexpression node)
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

        throw new Exception($"Unexpected bound node {node.Boundnodekind}");
    }
}
