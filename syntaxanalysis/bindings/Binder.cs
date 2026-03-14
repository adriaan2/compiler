using System.Linq.Expressions;
using bindings;
using syntaxer;
internal class Binder
    {
        public Boundexpression Bind(LiteralExpressionsyntax expressionsyntax)
    {
        switch (expressionsyntax.Kind)
        {
            
            case SyntaxKind.numberexpression:
                    return Bindnumberexpression((syntaxer.numberSyntax) expressionsyntax);
            case SyntaxKind.binaryexpression:
                    return Bindbinarysyntax((BynarySyntax)expressionsyntax);
            case SyntaxKind.unaryexpression:
                    return Bindunarysyntax((UnarySyntax) expressionsyntax);

            default:
             throw new Exception($"unkown syntax{expressionsyntax.Kind}");
        }
    }

    private  Boundexpression Bindunarysyntax(UnarySyntax expressionsyntax)
    {
       var Boundoperand= Bind(expressionsyntax.Operand);
       var Boundoperatorkind=bindunaryoperatorkind(expressionsyntax.OperatorToken.Kind);
       return new Boundunaryexpression(Boundoperatorkind,Boundoperand);

    }

   

    private Boundexpression Bindbinarysyntax(BynarySyntax expressionsyntax)
    {
        throw new NotImplementedException();
    }

    private Boundexpression Bindnumberexpression(numberSyntax expressionsyntax)
    {
     int value= expressionsyntax.Token.Value is not null?(int)expressionsyntax.Token.Value: 0;
     return new BoundNumberexpression(value);
    }
     private Boundnodekind  bindunaryoperatorkind(SyntaxKind kind)
    {
        throw new NotImplementedException();
    }
}