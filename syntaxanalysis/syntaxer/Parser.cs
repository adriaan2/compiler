namespace syntaxer;

public class Parser
{
    List<string> _diagnostics=new();
   public IEnumerable<string> Diagnostics=> _diagnostics;
    int _position=0;
    public int Position =>_position;
    readonly Syntaxtoken[] _tokens;
    public Parser(string Text)
    { 
        Lexer lexer=new Lexer(Text);
        Syntaxtoken syntaxtoken;
        List<Syntaxtoken> tokens =new();
        do
        {
             syntaxtoken= lexer.Lex();
             if (syntaxtoken.Kind!=SyntaxKind.whitespaceToken&& syntaxtoken.Kind!=SyntaxKind.badtoken)
             {
                tokens.Add(syntaxtoken);
             }

        } while (syntaxtoken.Kind!=SyntaxKind.endoffiletoken);
        _diagnostics.AddRange(lexer.Diagnostics);
        _tokens=tokens.ToArray();

    }
    private Syntaxtoken Peek(int offset)
    {
        var index = _position + offset;
        if (index >= _tokens.Length)
            return _tokens[_tokens.Length - 1];
        return _tokens[index];
        
    }
    private Syntaxtoken Nexttoken()
    {
        var current=Current;
        _position++;
        return current;
    }
    private Syntaxtoken Current => Peek(0);
    private Syntaxtoken match(SyntaxKind kind,string printable="")
    {
        if (Current.Kind==kind)
                return Nexttoken();
        System.Console.WriteLine(printable);
        _diagnostics.Add($"Error unexpected token in parser {Current.Kind}" );
        return new Syntaxtoken(kind, Current.POsition, null, null);
    }
    public LiteralExpressionsyntax parse()
    {
        return ParseExpression();
    }

    private LiteralExpressionsyntax ParseExpression()
    {
        System.Console.WriteLine("d");
        if (IsTypeKeyword(Current.Kind))
            return ParseVariableDeclaration();

        if (Current.Kind == SyntaxKind.identifierToken && Peek(1).Kind == SyntaxKind.equalsToken)
            return ParseAssignmentExpression();

        return ParseBinaryExpression();
    }

    private LiteralExpressionsyntax ParseVariableDeclaration()
    {
        var typeClause = ParseTypeClause();
        var identifier = match(SyntaxKind.identifierToken,"identifier");
        var equals = match(SyntaxKind.equalsToken,"equal");
        var initializer = ParseBinaryExpression();
        return new VariableDeclarationSyntax(typeClause, identifier, equals, initializer);
    }

    private TypeClauseSyntax ParseTypeClause()
    {
        var keyword = Nexttoken();
        Syntaxtoken? openBracket = null;
        Syntaxtoken? closeBracket = null;

        if (Current.Kind == SyntaxKind.openbracket && Peek(1).Kind == SyntaxKind.closebracket)
        {
            openBracket = Nexttoken();
            closeBracket = Nexttoken();
        }

        return new TypeClauseSyntax(keyword, openBracket, closeBracket);
    }

    private static bool IsTypeKeyword(SyntaxKind kind)
    {
        return kind == SyntaxKind.intKeyword || kind == SyntaxKind.boolKeyword||SyntaxKind.charkeyword==kind;
    }

    private LiteralExpressionsyntax ParseAssignmentExpression()
    {
        var identifier = match(SyntaxKind.identifierToken);
        var equals = match(SyntaxKind.equalsToken,"equals");
        var expression = ParseBinaryExpression();
        return new AssignmentExpressionSyntax(identifier, equals, expression);
    }

    private LiteralExpressionsyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        LiteralExpressionsyntax left;

        var unaryOperatorPrecedence = Precedence.GetUnaryOperatorPrecedence(Current.Kind);
        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence > parentPrecedence)
        {
            var operatorToken = Nexttoken();
            var operand = ParseBinaryExpression(unaryOperatorPrecedence);
            left = new UnarySyntax(operatorToken, operand);
        }
        else
        {
            left = ParsePostfixExpression();
        }

        while (true)
        {
            var precedence = Precedence.GetBinaryOperatorPrecedence(Current.Kind);
            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            var operatorToken = Nexttoken();
            var right = ParseBinaryExpression(precedence);
            left = new BynarySyntax(left, operatorToken, right);
        }

        return left;
    }

    private LiteralExpressionsyntax ParsePostfixExpression()
    {
        var left = ParsePrimaryexpression();

        while (Current.Kind == SyntaxKind.openbracket)
        {
            var openBracket = Nexttoken();
            var index = ParseExpression();
            var closeBracket = match(SyntaxKind.closebracket, "close bracket");
            left = new ArrayIndexExpressionSyntax(left, openBracket, index, closeBracket);
        }

        return left;
    }

    private LiteralExpressionsyntax ParsePrimaryexpression()
    {
        if (Current.Kind == SyntaxKind.openbracket)
        {
            return ParseArrayExpression();
        }
        if (Current.Kind==SyntaxKind.openparen)
        {
            var left=Nexttoken();
            var expression=ParseBinaryExpression();
            var right=match(SyntaxKind.closedparen,"127");
            return new Parenthessese(left, expression,right);

        }
        if (Current.Kind == SyntaxKind.trueKeyword || Current.Kind == SyntaxKind.falseKeyword)
        {
            var keywordToken = Nexttoken();
            return new BooleanSyntax(keywordToken);
        }
        if (Current.Kind == SyntaxKind.identifierToken)
        {
            var identifierToken = Nexttoken();
            return new NameExpressionSyntax(identifierToken);
        }
        if (Current.Kind==SyntaxKind.charvaltoken)
        {
            System.Console.WriteLine("d");
            var charToken = Nexttoken();
            var openQuote = new Syntaxtoken(SyntaxKind.badtoken, charToken.POsition, "'", null);
            var characterText = charToken.Value?.ToString();
            var characterValue = new Syntaxtoken(
                SyntaxKind.charvaltoken,
                charToken.POsition + 1,
                characterText,
                charToken.Value);
            var closeQuote = new Syntaxtoken(
                SyntaxKind.badtoken,
                charToken.POsition + (charToken.Text?.Length ?? 0) - 1,
                "'",
                   null);

            return new Charsyntax(openQuote, characterValue, closeQuote);
        }
        var numberToken =match(SyntaxKind.numberToken,"141");
        return new numberSyntax(numberToken);
    }

    private LiteralExpressionsyntax ParseArrayExpression()
    {
        var openBracket = match(SyntaxKind.openbracket, "open bracket");
        var elements = new List<LiteralExpressionsyntax>();

        if (Current.Kind != SyntaxKind.closebracket)
        {
            while (true)
            {
                elements.Add(ParseExpression());

                if (Current.Kind != SyntaxKind.commaToken)
                    break;

                Nexttoken();
            }
        }

        var closeBracket = match(SyntaxKind.closebracket, "close bracket");
        return new ArrayExpressionSyntax(openBracket, elements, closeBracket);
    }
}
