namespace syntaxer;


using System.Security;

public enum SyntaxKind
{
    numberToken,
    trueKeyword,
    falseKeyword,
    equalsEqualsToken,
    plusToken,
    minusToken,
    slashtoken,
    bangToken,
    ampersandAmpersandToken,
    pipePipeToken,
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
    errorexpression,
    bracketexpression,
    unaryexpression,
    booleanexpression}

public class Syntaxtoken: SyntaxNode
{


    public Syntaxtoken(SyntaxKind kind, int position, string? text, object? value)
    {
        Kind = kind;
        POsition = position;
        Text = text;
        Value = value;
    }
    public override SyntaxKind Kind { get; }
    public int POsition { get; }
    public string? Text { get; }
    public object? Value { get; }

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
public abstract class LiteralExpressionsyntax : SyntaxNode
{

}
sealed class numberSyntax : LiteralExpressionsyntax
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
sealed class BooleanSyntax : LiteralExpressionsyntax
{
    public BooleanSyntax(Syntaxtoken keywordToken)
    {
        KeywordToken = keywordToken;
    }

    public Syntaxtoken KeywordToken { get; }
    public override SyntaxKind Kind => SyntaxKind.booleanexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return KeywordToken;
    }
}
 sealed class BynarySyntax : LiteralExpressionsyntax
    {
        public BynarySyntax(LiteralExpressionsyntax left, Syntaxtoken operatortoken, LiteralExpressionsyntax right)
        {
            Left = left;
            OperatorToken = operatortoken;
            Right = right;
        }
        
        public override SyntaxKind Kind => SyntaxKind.binaryexpression;
    
        public LiteralExpressionsyntax Left { get; }
        public Syntaxtoken OperatorToken { get; }
        public LiteralExpressionsyntax Right { get; }
        public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }

    }


sealed class ErrorSyntax : LiteralExpressionsyntax
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

