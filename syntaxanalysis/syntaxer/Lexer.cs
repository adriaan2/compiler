
namespace syntaxer;


public class Lexer
{
   private readonly string _text;
   private int _position; 
   List<string> _diagnostics=new();
   public IEnumerable<string> Diagnostics=> _diagnostics;
   public Lexer(string text ){
        _text=text;
    }
    private char Current
    {
        get
        {
            if (_position== _text.Length)
            {
               return '\0';
            }
            return _text[_position];
        }

    }
    private char Lookahead
    {
        get
        {
            var index = _position + 1;
            if (index >= _text.Length)
                return '\0';

            return _text[index];
        }
    }
    private void Next()
    {
        _position++;
      //  System.Console.WriteLine(_position);
    }
    
           
    public Syntaxtoken Lex()
    {
        //  
        if (_position>= _text.Length )
        {
            return new Syntaxtoken(SyntaxKind.endoffiletoken, _position, "\0" , null);
        }
        if (char.IsLetter(Current))
        {
            var start = _position;
            while (char.IsLetter(Current))
            {
                Next();
            }

            var length = _position - start;
            var text = _text.Substring(start, length);
            
            var kind = text switch
            {
                
                "char"=> SyntaxKind.charkeyword,
                "true" => SyntaxKind.trueKeyword,
                "false" => SyntaxKind.falseKeyword,
                "int" => SyntaxKind.intKeyword,
                "bool" => SyntaxKind.boolKeyword,
                _ => SyntaxKind.identifierToken
            };

            object? value = kind switch
            {
                SyntaxKind.trueKeyword => true,
                SyntaxKind.falseKeyword => false,
                _ => null
            };

            return new Syntaxtoken(kind, start, text, value);
        }
        if (char.IsDigit(Current))
        {
            //System.Console.WriteLine("digit");
            var start= _position;
            while(char.IsDigit(Current))
            {
                Next();
            }

            var length=_position-start;
            var text=_text.Substring(start,length);
            if (!int.TryParse(text,out var value ))
            {
                
            _diagnostics.Add($"Number {_text} to big for int32'");
            }
            return new Syntaxtoken(SyntaxKind.numberToken, start, text, value);
            }



        
        if (char.IsWhiteSpace(Current))
        {
            var start= _position;
            while(char.IsWhiteSpace(Current))
            {
                Next();
            }

            var length=_position-start;
            var text=_text.Substring(start,length);
            return new Syntaxtoken(SyntaxKind.whitespaceToken, start, text, null);
            
        }
        if (Current=='+')
        {
        return new Syntaxtoken(SyntaxKind.plusToken, _position++, "+", null);
                        
        }
        else if (Current=='[')
        {
            return new Syntaxtoken(SyntaxKind.openbracket, _position++, "[", null);
        }
        else if (Current==']')
        {
            return new Syntaxtoken(SyntaxKind.closebracket, _position++, "]", null);
        }
        else if (Current==',')
        {
            return new Syntaxtoken(SyntaxKind.commaToken, _position++, ",", null);
        }
        else if (Current=='=' && Lookahead!='=')
        {
            return new Syntaxtoken(SyntaxKind.equalsToken, _position++, "=", null);
        }
        else if (Current=='=' && Lookahead=='=')
        {
                    var position = _position;
                    _position += 2;
                    return new Syntaxtoken(SyntaxKind.equalsEqualsToken, position, "==", null);
        }
        else if (Current=='-')
        {
                    return new Syntaxtoken(SyntaxKind.minusToken, _position++, "-", null);

        }
        else if (Current=='/')
        {
                    return new Syntaxtoken(SyntaxKind.slashtoken, _position++, "/", null);

        }
        else  if (Current=='(')
        {
                   return new Syntaxtoken(SyntaxKind.openparen, _position++, "(", null);

        }
        else  if (Current==')')
        {
                    return new Syntaxtoken(SyntaxKind.closedparen, _position++, ")", null);

        }
        else  if (Current=='*')
        {
                    return new Syntaxtoken(SyntaxKind.timestoken, _position++, "*", null);

        }
        else if (Current=='!')
        {
                    return new Syntaxtoken(SyntaxKind.bangToken, _position++, "!", null);
        }
        else if (Current=='&' && Lookahead=='&')
        {
                    var position = _position;
                    _position += 2;
                    return new Syntaxtoken(SyntaxKind.ampersandAmpersandToken, position, "&&", null);
        }
        else if (Current=='|' && Lookahead=='|')
        {
                    var position = _position;
                    _position += 2;
                    return new Syntaxtoken(SyntaxKind.pipePipeToken, position, "||", null);
        }
        
       else if (Current == '\'')
{
    var start = _position;

    // Move past opening quote
    Next();

    if (Current == '\0')
    {
        _diagnostics.Add("Unterminated char literal.");
        return new Syntaxtoken(SyntaxKind.badtoken, start, "'", null);
    }

    var charValue = Current;

    Next();

    if (Current != '\'')
    {
        _diagnostics.Add("Char literal must contain exactly one character.");
        return new Syntaxtoken(SyntaxKind.badtoken, start, _text.Substring(start, _position - start), null);
    }

    // Move past closing quote
    Next();

    var text = _text.Substring(start, _position - start);

    return new Syntaxtoken(SyntaxKind.charvaltoken, start, text, charValue);
}

       
        _diagnostics.Add($"error bad character input in lexer line 171 {Current}" );
        return new Syntaxtoken(SyntaxKind.badtoken, _position++, _text.Substring(_position-1,1),null );
        

    }

    
   
}
