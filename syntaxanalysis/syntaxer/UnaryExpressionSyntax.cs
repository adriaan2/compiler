namespace syntaxer;

sealed class UnarySyntax : LiteralExpressionsyntax
{
    public UnarySyntax(Syntaxtoken operatorToken, LiteralExpressionsyntax operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public Syntaxtoken OperatorToken { get; }
    public LiteralExpressionsyntax Operand { get; }
    public override SyntaxKind Kind => SyntaxKind.unaryexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}

