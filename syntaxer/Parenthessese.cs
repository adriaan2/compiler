using System.Threading.Channels;

sealed class Parenthessese:ExpressionSyntax
{
    public Parenthessese(Syntaxtoken openparenthesis, ExpressionSyntax expression, Syntaxtoken closeparenthesis )
    {
        Openparenthesis=openparenthesis;
        Closeparenthesis=closeparenthesis;
        Expression=expression;
    }

    public Syntaxtoken Openparenthesis { get;  }
    public Syntaxtoken Closeparenthesis { get; }
    public ExpressionSyntax Expression { get; }

    public override SyntaxKind Kind => SyntaxKind.bracketexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Openparenthesis;
        yield return Expression;
        yield return Closeparenthesis;
    }
}
