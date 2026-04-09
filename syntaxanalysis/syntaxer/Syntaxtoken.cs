namespace syntaxer;


using System.Security;

public enum SyntaxKind
{
    arraytoken,
    iftoken,
    charkeyword,
    numberToken,
    openbracket,
    closebracket,
    commaToken,
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
    typeclause,
    unaryexpression,
    booleanexpression,
    charvaltoken,
    arrayexpression,
    arrayindexexpression
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
sealed class TypeClauseSyntax : SyntaxNode
{
    public TypeClauseSyntax(Syntaxtoken keyword, Syntaxtoken? openBracketToken = null, Syntaxtoken? closeBracketToken = null)
    {
        Keyword = keyword;
        OpenBracketToken = openBracketToken;
        CloseBracketToken = closeBracketToken;
    }

    public Syntaxtoken Keyword { get; }
    public Syntaxtoken? OpenBracketToken { get; }
    public Syntaxtoken? CloseBracketToken { get; }
    public bool IsArray => OpenBracketToken is not null && CloseBracketToken is not null;

    public override SyntaxKind Kind => SyntaxKind.typeclause;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Keyword;

        if (OpenBracketToken is not null)
            yield return OpenBracketToken;

        if (CloseBracketToken is not null)
            yield return CloseBracketToken;
    }
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
    public VariableDeclarationSyntax(TypeClauseSyntax typeClause, Syntaxtoken identifier, Syntaxtoken equalsToken, LiteralExpressionsyntax initializer)
    {
        TypeClause = typeClause;
        Identifier = identifier;
        EqualsToken = equalsToken;
        Initializer = initializer;
    }

    public TypeClauseSyntax TypeClause { get; }
    public Syntaxtoken Identifier { get; }
    public Syntaxtoken EqualsToken { get; }
    public LiteralExpressionsyntax Initializer { get; }

    public override SyntaxKind Kind => SyntaxKind.variabledeclarationexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return TypeClause;
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

sealed class ArrayExpressionSyntax : LiteralExpressionsyntax
{
    public ArrayExpressionSyntax(Syntaxtoken openBracketToken, IReadOnlyList<LiteralExpressionsyntax> elements, Syntaxtoken closeBracketToken)
    {
        OpenBracketToken = openBracketToken;
        Elements = elements;
        CloseBracketToken = closeBracketToken;
    }

    public Syntaxtoken OpenBracketToken { get; }
    public IReadOnlyList<LiteralExpressionsyntax> Elements { get; }
    public Syntaxtoken CloseBracketToken { get; }

    public override SyntaxKind Kind => SyntaxKind.arrayexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return OpenBracketToken;

        foreach (var element in Elements)
            yield return element;

        yield return CloseBracketToken;
    }
}

sealed class ArrayIndexExpressionSyntax : LiteralExpressionsyntax
{
    public ArrayIndexExpressionSyntax(LiteralExpressionsyntax target, Syntaxtoken openBracketToken, LiteralExpressionsyntax index, Syntaxtoken closeBracketToken)
    {
        Target = target;
        OpenBracketToken = openBracketToken;
        Index = index;
        CloseBracketToken = closeBracketToken;
    }

    public LiteralExpressionsyntax Target { get; }
    public Syntaxtoken OpenBracketToken { get; }
    public LiteralExpressionsyntax Index { get; }
    public Syntaxtoken CloseBracketToken { get; }

    public override SyntaxKind Kind => SyntaxKind.arrayindexexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Target;
        yield return OpenBracketToken;
        yield return Index;
        yield return CloseBracketToken;
    }
}
