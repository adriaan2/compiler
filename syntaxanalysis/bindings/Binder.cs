using System.Linq.Expressions;
using bindings;
using syntaxer;
internal class Binder
    {
        private readonly List<string> _diagnostocs=new();
        public IEnumerable<string> Diagnostics=>_diagnostocs;
        public Boundexpression Bind(LiteralExpressionsyntax expressionsyntax)
    {
        switch (expressionsyntax.Kind)
        {
            
            case SyntaxKind.numberexpression:
                    return Bindnumberexpression((syntaxer.numberSyntax) expressionsyntax);
            case SyntaxKind.booleanexpression:
                    return Bindbooleanexpression((BooleanSyntax)expressionsyntax);
            case SyntaxKind.binaryexpression:
                    return Bindbinarysyntax((BynarySyntax)expressionsyntax);
            case SyntaxKind.unaryexpression:
                    return Bindunarysyntax((UnarySyntax) expressionsyntax);
            case SyntaxKind.bracketexpression:
                    return Bindparenthesizedsyntax((Parenthessese)expressionsyntax);

            default:
             throw new Exception($"unkown syntax{expressionsyntax.Kind}");
        }
    }

    private  Boundexpression Bindunarysyntax(UnarySyntax expressionsyntax)
    {
       var Boundoperand= Bind(expressionsyntax.Operand);
       var Boundoperatorkind=bindunaryoperatorkind(expressionsyntax.OperatorToken.Kind);
       if (!IsValidUnaryOperator(Boundoperatorkind, Boundoperand.Type))
       {
            _diagnostocs.Add($"Unary operator '{expressionsyntax.OperatorToken.Text}' is not defined for type {Boundoperand.Type.Name}");
            return Boundoperand;
       }
       return new Boundunaryexpression(Boundoperatorkind, Boundoperand);

    }

    private Boundexpression Bindparenthesizedsyntax(Parenthessese expressionsyntax)
    {
        return Bind(expressionsyntax.Expression);
    }

   

    private Boundexpression Bindbinarysyntax(BynarySyntax expressionsyntax)
    {
        var left= Bind(expressionsyntax.Left);
        var right=Bind(expressionsyntax.Right);
       

       var Boundoperatorkind=bindbinaryoperatorkind(expressionsyntax.OperatorToken.Kind);
       if (!IsValidBinaryOperator(Boundoperatorkind, left.Type, right.Type))
       {
            _diagnostocs.Add($"Binary operator '{expressionsyntax.OperatorToken.Text}' is not defined for types {left.Type.Name} and {right.Type.Name}");
            return left;
       }
       
       return new BoundBinaryexpression(left,Boundoperatorkind,right);
    }

    private BoundBinaryoperatorkind bindbinaryoperatorkind(SyntaxKind kind)
    {
       
        
        switch (kind)
        {
            case SyntaxKind.plusToken:
                    return BoundBinaryoperatorkind.addition;
            case SyntaxKind.minusToken:
                    return BoundBinaryoperatorkind.subtraction;
            case SyntaxKind.timestoken:
                    return BoundBinaryoperatorkind.times;
            case SyntaxKind.slashtoken:
                    return BoundBinaryoperatorkind.division;
            case SyntaxKind.equalsEqualsToken:
                    return BoundBinaryoperatorkind.Equals;
            case SyntaxKind.ampersandAmpersandToken:
                    return BoundBinaryoperatorkind.LogicalAnd;
            case SyntaxKind.pipePipeToken:
                    return BoundBinaryoperatorkind.LogicalOr;
            
            default:

             throw new Exception($"unexpected binary syntax{kind}");
        }
    }

    private Boundexpression Bindnumberexpression(numberSyntax expressionsyntax)
    {
        
     int value= expressionsyntax.Token.Value is int?(int)expressionsyntax.Token.Value: 0;
     return new BoundNumberexpression(value);
    }
    private Boundexpression Bindbooleanexpression(BooleanSyntax expressionsyntax)
    {
        bool value = expressionsyntax.KeywordToken.Value is bool booleanValue && booleanValue;
        return new BoundBooleanexpression(value);
    }
     private Boundunaryoperatorkind  bindunaryoperatorkind(SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.plusToken:
                    return Boundunaryoperatorkind.Identity;
            case SyntaxKind.minusToken:
                return Boundunaryoperatorkind.Negation;
            case SyntaxKind.bangToken:
                return Boundunaryoperatorkind.LogicalNegation;
            default:
             throw new Exception($"{kind} not a unary operation");
        }
    }

    private static bool IsValidUnaryOperator(Boundunaryoperatorkind operatorKind, Type operandType)
    {
        if (operandType == typeof(int))
            return operatorKind == Boundunaryoperatorkind.Identity || operatorKind == Boundunaryoperatorkind.Negation;

        if (operandType == typeof(bool))
            return operatorKind == Boundunaryoperatorkind.LogicalNegation;

        return false;
    }

    private static bool IsValidBinaryOperator(BoundBinaryoperatorkind operatorKind, Type leftType, Type rightType)
    {
        if (leftType != rightType)
            return false;

        if (leftType == typeof(int))
        {
            return operatorKind == BoundBinaryoperatorkind.addition
                || operatorKind == BoundBinaryoperatorkind.subtraction
                || operatorKind == BoundBinaryoperatorkind.times
                || operatorKind == BoundBinaryoperatorkind.division
                || operatorKind == BoundBinaryoperatorkind.Equals;
        }

        if (leftType == typeof(bool))
        {
            return operatorKind == BoundBinaryoperatorkind.Equals
                || operatorKind == BoundBinaryoperatorkind.LogicalAnd
                || operatorKind == BoundBinaryoperatorkind.LogicalOr;
        }

        return false;
    }
}
