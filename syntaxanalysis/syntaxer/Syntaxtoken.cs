namespace syntaxer;


using System.Security;

public enum SyntaxKind
{
    charkeyword,
    numberToken,
    identifierToken,
    trueKeyword,
    falseKeyword,
    intKeyword,
    boolKeyword,
    equalsToken,
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
    nameexpression,
    assignmentexpression,
    variabledeclarationexpression,
    binaryexpression,
    timestoken,
    errorexpression,
    bracketexpression,
    unaryexpression,
    booleanexpression,
    charvaltoken
    }

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
sealed class NameExpressionSyntax : LiteralExpressionsyntax
{
    public NameExpressionSyntax(Syntaxtoken identifierToken)
    {
        IdentifierToken = identifierToken;
    }

    public Syntaxtoken IdentifierToken { get; }
    public override SyntaxKind Kind => SyntaxKind.nameexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return IdentifierToken;
    }
}

sealed class AssignmentExpressionSyntax : LiteralExpressionsyntax
{
    public AssignmentExpressionSyntax(Syntaxtoken identifierToken, Syntaxtoken equalsToken, LiteralExpressionsyntax expression)
    {
        IdentifierToken = identifierToken;
        EqualsToken = equalsToken;
        Expression = expression;
    }

    public Syntaxtoken IdentifierToken { get; }
    public Syntaxtoken EqualsToken { get; }
    public LiteralExpressionsyntax Expression { get; }

    public override SyntaxKind Kind => SyntaxKind.assignmentexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return IdentifierToken;
        yield return EqualsToken;
        yield return Expression;
    }
}

sealed class VariableDeclarationSyntax : LiteralExpressionsyntax
{
    public VariableDeclarationSyntax(Syntaxtoken keyword, Syntaxtoken identifier, Syntaxtoken equalsToken, LiteralExpressionsyntax initializer)
    {
        Keyword = keyword;
        Identifier = identifier;
        EqualsToken = equalsToken;
        Initializer = initializer;
    }

    public Syntaxtoken Keyword { get; }
    public Syntaxtoken Identifier { get; }
    public Syntaxtoken EqualsToken { get; }
    public LiteralExpressionsyntax Initializer { get; }

    public override SyntaxKind Kind => SyntaxKind.variabledeclarationexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Keyword;
        yield return Identifier;
        yield return EqualsToken;
        yield return Initializer;
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

