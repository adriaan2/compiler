namespace syntaxer;

public class Evaluator
{
    readonly LiteralExpressionsyntax _root;
    public Evaluator(LiteralExpressionsyntax root)
    {
        _root=root;
    }
    public int Evaluate()
    {
        return evaluateexpressionroot(_root);
    }
     int evaluateexpressionroot(LiteralExpressionsyntax expression)
    {
        if (expression is numberSyntax n)
        {
            return (int) n.Token.Value;
        }

        if (expression is BynarySyntax b)
        {
            var left = evaluateexpressionroot(b.Left);
            var right = evaluateexpressionroot(b.Right);

            switch (b.OperatorToken.Kind)
            {
                case SyntaxKind.plusToken:
                    return left + right;
                case SyntaxKind.minusToken:
                    return left - right;
                case SyntaxKind.timestoken:
                    return left * right;
                case SyntaxKind.slashtoken:
                    return left / right;
                default:
                    throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }
        }

        if (expression is UnarySyntax u)
        {
            var operand = evaluateexpressionroot(u.Operand);

            switch (u.OperatorToken.Kind)
            {
                case SyntaxKind.plusToken:
                    return operand;
                case SyntaxKind.minusToken:
                    return -operand;
                default:
                    throw new Exception($"Unexpected unary operator {u.OperatorToken.Kind}");
            }
        }

        if (expression is Parenthessese p)
        {
            return evaluateexpressionroot(p.Expression);
        }

      

        throw new Exception($"Unexpected node {expression.Kind}");
    }

}

