using Microsoft.VisualBasic;

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
        _diagnostics.Add($"error bad character input {Current}" );
        return new Syntaxtoken(SyntaxKind.badtoken, _position++, _text.Substring(_position-1,1),null );


    }

    
   
}
