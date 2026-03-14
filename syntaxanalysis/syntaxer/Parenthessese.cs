using System.Threading.Channels;

namespace syntaxer;


sealed class Parenthessese:LiteralExpressionsyntax
{
    public Parenthessese(Syntaxtoken openparenthesis, LiteralExpressionsyntax expression, Syntaxtoken closeparenthesis )
    {
        Openparenthesis=openparenthesis;
        Closeparenthesis=closeparenthesis;
        Expression=expression;
    }

    public Syntaxtoken Openparenthesis { get;  }
    public Syntaxtoken Closeparenthesis { get; }
    public LiteralExpressionsyntax Expression { get; }

    public override SyntaxKind Kind => SyntaxKind.bracketexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Openparenthesis;
        yield return Expression;
        yield return Closeparenthesis;
    }
}

