using bindings;
using syntaxer;

internal class Binder
{
    private readonly Dictionary<string, VariableSymbol> _variables;
    private readonly List<string> _diagnostocs = new();

    public IEnumerable<string> Diagnostics => _diagnostocs;

    public Binder(Dictionary<string, VariableSymbol> variables)
    {
        _variables = variables;
    }

    public Boundexpression Bind(LiteralExpressionsyntax expressionsyntax)
    {
        switch (expressionsyntax.Kind)
        {
            case SyntaxKind.numberexpression:
                return Bindnumberexpression((numberSyntax)expressionsyntax);
            case SyntaxKind.booleanexpression:
                return Bindbooleanexpression((BooleanSyntax)expressionsyntax);
            case SyntaxKind.binaryexpression:
                return Bindbinarysyntax((BynarySyntax)expressionsyntax);
            case SyntaxKind.unaryexpression:
                return Bindunarysyntax((UnarySyntax)expressionsyntax);
            case SyntaxKind.bracketexpression:
                return Bindparenthesizedsyntax((Parenthessese)expressionsyntax);
            case SyntaxKind.nameexpression:
                return Bindnameexpression((NameExpressionSyntax)expressionsyntax);
            case SyntaxKind.assignmentexpression:
                return Bindassignmentexpression((AssignmentExpressionSyntax)expressionsyntax);
            case SyntaxKind.variabledeclarationexpression:
                return Bindvariabledeclaration((VariableDeclarationSyntax)expressionsyntax);
            case SyntaxKind.charvaltoken:
                return bindcharexpression((Charsyntax)expressionsyntax);
            case SyntaxKind.arrayexpression:
                return Bindarrayexpression((ArrayExpressionSyntax)expressionsyntax);
            case SyntaxKind.arrayindexexpression:
                return Bindarrayindexexpression((ArrayIndexExpressionSyntax)expressionsyntax);
            default:
                throw new Exception($"unkown syntax{expressionsyntax.Kind}");
        }
    }

    private Boundexpression Bindunarysyntax(UnarySyntax expressionsyntax)
    {
        var boundOperand = Bind(expressionsyntax.Operand);
        var boundOperatorKind = bindunaryoperatorkind(expressionsyntax.OperatorToken.Kind);
        if (!IsValidUnaryOperator(boundOperatorKind, boundOperand.Type))
        {
            _diagnostocs.Add($"Unary operator '{expressionsyntax.OperatorToken.Text}' is not defined for type {boundOperand.Type.Name}");
            return boundOperand;
        }

        return new Boundunaryexpression(boundOperatorKind, boundOperand);
    }

    private Boundexpression Bindparenthesizedsyntax(Parenthessese expressionsyntax)
    {
        return Bind(expressionsyntax.Expression);
    }

    private Boundexpression Bindnameexpression(NameExpressionSyntax expressionsyntax)
    {
        var name = expressionsyntax.IdentifierToken.Text ?? string.Empty;
        if (!_variables.TryGetValue(name, out var variable))
        {
            _diagnostocs.Add($"Variable '{name}' does not exist.");
            return new BoundNumberexpression(0);
        }

        return new BoundVariableexpression(variable);
    }

    private Boundexpression Bindassignmentexpression(AssignmentExpressionSyntax expressionsyntax)
    {
        var name = expressionsyntax.IdentifierToken.Text ?? string.Empty;
        var boundExpression = Bind(expressionsyntax.Expression);

        if (!_variables.TryGetValue(name, out var variable))
        {
            _diagnostocs.Add($"Variable '{name}' does not exist.");
            return boundExpression;
        }

        if (!AreTypesCompatible(variable.Type, variable.ElementType, boundExpression))
        {
            _diagnostocs.Add($"Cannot assign value of type {GetDisplayTypeName(boundExpression.Type, GetArrayElementType(boundExpression))} to variable '{name}' of type {GetDisplayTypeName(variable.Type, variable.ElementType)}.");
            return boundExpression;
        }

        return new BoundAssignmentexpression(variable, boundExpression);
    }

    private Boundexpression Bindvariabledeclaration(VariableDeclarationSyntax expressionsyntax)
    {
        var name = expressionsyntax.Identifier.Text ?? string.Empty;
        var initializer = Bind(expressionsyntax.Initializer);
        var (variableType, elementType) = BindTypeClause(expressionsyntax.TypeClause);

        if (_variables.ContainsKey(name))
        {
            _diagnostocs.Add($"Variable '{name}' is already declared.");
            return initializer;
        }

        if (!AreTypesCompatible(variableType, elementType, initializer))
        {
            _diagnostocs.Add($"Variable '{name}' must be of type {GetDisplayTypeName(variableType, elementType)}.");
            return initializer;
        }

        var variable = new VariableSymbol(name, variableType, elementType);
        _variables.Add(name, variable);
        return new BoundVariabledeclarationexpression(variable, initializer);
    }

    private static (Type Type, Type? ElementType) BindTypeClause(TypeClauseSyntax typeClause)
    {
        var baseType = typeClause.Keyword.Kind switch
        {
            SyntaxKind.intKeyword => typeof(int),
            SyntaxKind.boolKeyword => typeof(bool),
            SyntaxKind.charkeyword => typeof(char),
            _ => throw new Exception($"Unexpected type keyword {typeClause.Keyword.Kind}")
        };

        if (typeClause.IsArray)
            return (typeof(ArrayValue), baseType);

        return (baseType, null);
    }

    private Boundexpression Bindbinarysyntax(BynarySyntax expressionsyntax)
    {
        var left = Bind(expressionsyntax.Left);
        var right = Bind(expressionsyntax.Right);

        var boundOperatorKind = bindbinaryoperatorkind(expressionsyntax.OperatorToken.Kind);
        if (!IsValidBinaryOperator(boundOperatorKind, left.Type, right.Type))
        {
            _diagnostocs.Add($"Binary operator '{expressionsyntax.OperatorToken.Text}' is not defined for types {left.Type.Name} and {right.Type.Name}");
            return left;
        }

        return new BoundBinaryexpression(left, boundOperatorKind, right);
    }

    private Boundexpression Bindarrayexpression(ArrayExpressionSyntax expressionsyntax)
    {
        var elements = expressionsyntax.Elements.Select(Bind).ToArray();
        if (elements.Length == 0)
        {
            _diagnostocs.Add("Array literals must contain at least one element.");
            return new BoundArrayexpression(typeof(object), Array.Empty<Boundexpression>());
        }

        var elementType = elements[0].Type;
        var arrayElementType = GetArrayElementType(elements[0]);

        for (var i = 1; i < elements.Length; i++)
        {
            if (!HaveSameType(elements[0], elements[i]))
            {
                _diagnostocs.Add("All array literal elements must have the same type.");
                return elements[0];
            }
        }

        if (elementType == typeof(ArrayValue) && arrayElementType is null)
        {
            _diagnostocs.Add("Array literal element type could not be determined.");
            return elements[0];
        }

        return new BoundArrayexpression(elementType == typeof(ArrayValue) ? arrayElementType! : elementType, elements);
    }

    private Boundexpression Bindarrayindexexpression(ArrayIndexExpressionSyntax expressionsyntax)
    {
        var array = Bind(expressionsyntax.Target);
        var index = Bind(expressionsyntax.Index);

        if (array.Type != typeof(ArrayValue))
        {
            _diagnostocs.Add("Indexing is only supported on arrays.");
            return array;
        }

        if (index.Type != typeof(int))
        {
            _diagnostocs.Add("Array indexes must be of type Int32.");
            return array;
        }

        var elementType = GetArrayElementType(array);
        if (elementType is null)
        {
            _diagnostocs.Add("Array element type could not be determined.");
            return array;
        }

        return new BoundArrayindexexpression(array, index, elementType);
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
        int value = expressionsyntax.Token.Value is int ? (int)expressionsyntax.Token.Value : 0;
        return new BoundNumberexpression(value);
    }

    private Boundcharexpression bindcharexpression(Charsyntax charsyntax)
    {
        var value = charsyntax.CharacterValue.Value is char character ? character : '\0';
        return new Boundcharexpression(value);
    }

    private Boundexpression Bindbooleanexpression(BooleanSyntax expressionsyntax)
    {
        bool value = expressionsyntax.KeywordToken.Value is bool booleanValue && booleanValue;
        return new BoundBooleanexpression(value);
    }

    private Boundunaryoperatorkind bindunaryoperatorkind(SyntaxKind kind)
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
                throw new Exception($"{kind} not a unary operation in binder.cs line 90");
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

        if (leftType == typeof(char))
            return operatorKind == BoundBinaryoperatorkind.Equals;

        return false;
    }

    private static bool AreTypesCompatible(Type declaredType, Type? declaredElementType, Boundexpression expression)
    {
        if (declaredType != expression.Type)
            return false;

        if (declaredType != typeof(ArrayValue))
            return true;

        return declaredElementType == GetArrayElementType(expression);
    }

    private static bool HaveSameType(Boundexpression left, Boundexpression right)
    {
        if (left.Type != right.Type)
            return false;

        if (left.Type != typeof(ArrayValue))
            return true;

        return GetArrayElementType(left) == GetArrayElementType(right);
    }

    private static Type? GetArrayElementType(Boundexpression expression)
    {
        return expression switch
        {
            BoundArrayexpression arrayExpression => arrayExpression.ElementType,
            BoundVariableexpression variableExpression => variableExpression.Variable.ElementType,
            BoundVariabledeclarationexpression declarationExpression => declarationExpression.Variable.ElementType,
            BoundAssignmentexpression assignmentExpression => assignmentExpression.Variable.ElementType,
            _ => null
        };
    }

    private static string GetDisplayTypeName(Type type, Type? elementType)
    {
        if (type == typeof(ArrayValue) && elementType is not null)
            return $"{elementType.Name}[]";

        return type.Name;
    }
}
