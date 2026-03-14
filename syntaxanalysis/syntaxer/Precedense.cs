namespace syntaxer;

public static class Precedence
{
     public static int GetBinaryOperatorPrecedence(SyntaxKind kind)
    {
        if (kind == SyntaxKind.timestoken || kind == SyntaxKind.slashtoken)
            return 2;
        if (kind == SyntaxKind.plusToken || kind == SyntaxKind.minusToken)
            return 1;
        return 0;
    }

    public static int GetUnaryOperatorPrecedence(SyntaxKind kind)
    {
        if (kind == SyntaxKind.plusToken || kind == SyntaxKind.minusToken)
            return 3;
        return 0;
    }
}
