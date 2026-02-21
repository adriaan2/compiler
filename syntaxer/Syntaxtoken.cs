
using System.Security;

public enum SyntaxKind
{
    numberToken,
    plusToken,
    minusToken,
    slashtoken,
    opencurly,
    closedcurly,
    openparen,
    closedparen,
    badtoken,
    endoffiletoken,
    whitespaceToken,
    numberexpression,
    binaryexpression,
    timestoken,
    errorexpression
    
}

public class Syntaxtoken: SyntaxNode
{


    public Syntaxtoken(SyntaxKind kind, int position, string text, object value)
    {
        Kind = kind;
        POsition = position;
        Text = text;
        Value = value;
    }
    public override SyntaxKind Kind { get; }
    public int POsition { get; }
    public string Text { get; }
    public object Value { get; }

    public override IEnumerable<SyntaxNode> getchildren()
    {
        return Enumerable.Empty<SyntaxNode>();
    }
}
public abstract class SyntaxNode
{

    public abstract SyntaxKind Kind { get; }
    public abstract IEnumerable<SyntaxNode> getchildren();
}
public abstract class ExpressionSyntax : SyntaxNode
{

}
sealed class numberSyntax : ExpressionSyntax
{
    public numberSyntax(Syntaxtoken token)
    {
        Token = token;
    }
    public override SyntaxKind Kind => SyntaxKind.numberexpression;
    public Syntaxtoken Token { get; }
    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Token;
    }

}
 sealed class BynarySyntax : ExpressionSyntax
    {
        public BynarySyntax(ExpressionSyntax left, Syntaxtoken operatortoken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatortoken;
            Right = right;
        }
        public override SyntaxKind Kind => SyntaxKind.binaryexpression;
    
        public ExpressionSyntax Left { get; }
        public Syntaxtoken OperatorToken { get; }
        public ExpressionSyntax Right { get; }
        public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }

    }

sealed class ErrorSyntax : ExpressionSyntax
{
    public ErrorSyntax(Syntaxtoken token)
    {
        Token = token;
    }

    public Syntaxtoken Token { get; }
    public override SyntaxKind Kind => SyntaxKind.errorexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Token;
    }
}
