namespace syntaxer;

public static class Precedence
{
     public static int GetBinaryOperatorPrecedence(SyntaxKind kind)
    {
        if (kind == SyntaxKind.pipePipeToken)
            return 1;
        if (kind == SyntaxKind.ampersandAmpersandToken)
            return 2;
        if (kind == SyntaxKind.equalsEqualsToken)
            return 3;
        if (kind == SyntaxKind.timestoken || kind == SyntaxKind.slashtoken)
            return 5;
        if (kind == SyntaxKind.plusToken || kind == SyntaxKind.minusToken)
            return 4;
        return 0;
    }

    public static int GetUnaryOperatorPrecedence(SyntaxKind kind)
    {
        if (kind == SyntaxKind.plusToken || kind == SyntaxKind.minusToken || kind == SyntaxKind.bangToken)
            return 6;
        return 0;
    }
}
