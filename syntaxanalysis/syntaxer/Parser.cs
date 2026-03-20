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
    private Syntaxtoken match(SyntaxKind kind)
    {
        if (Current.Kind==kind)
                return Nexttoken();
        _diagnostics.Add($"Error unexpected token {Current.Kind}");
        return new Syntaxtoken(kind, Current.POsition, null, null);
    }
    public LiteralExpressionsyntax parse()
    {
        return ParseBinaryExpression();
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
            left = ParsePrimaryexpression();
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

   

    private LiteralExpressionsyntax ParsePrimaryexpression()
    {
        if (Current.Kind==SyntaxKind.openparen)
        {
            var left=Nexttoken();
            var expression=ParseBinaryExpression();
            var right=match(SyntaxKind.closedparen);
            return new Parenthessese(left, expression,right);

        }
        if (Current.Kind == SyntaxKind.trueKeyword || Current.Kind == SyntaxKind.falseKeyword)
        {
            var keywordToken = Nexttoken();
            return new BooleanSyntax(keywordToken);
        }
        var numberToken =match(SyntaxKind.numberToken);
        return new numberSyntax(numberToken);
    }
}
