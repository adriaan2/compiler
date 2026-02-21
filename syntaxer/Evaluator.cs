public class Evaluator
{
    readonly ExpressionSyntax _root;
    public Evaluator(ExpressionSyntax root)
    {
        _root=root;
    }
    public int Evaluate()
    {
        return evaluateexpressionroot(_root);
    }
     int evaluateexpressionroot(ExpressionSyntax expression)
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

      

        throw new Exception($"Unexpected node {expression.Kind}");
    }

}
