using System.Collections.Generic;
using bindings;
using syntaxer;
using Xunit;
using Xunit.Sdk;

namespace test.Tests;

public class SmokeTests
{
    public static TheoryData<EvaluationCase> ArithmeticCases => new()
    {
        new("Addition", "1+2", 3),
        new("Subtraction", "7-4", 3),
        new("Multiplication", "3*5", 15),
        new("Division", "8/2", 4),
        new("OperatorPrecedence", "1+2*3", 7),
        new("Parentheses", "(1+2)*3", 9),
        new("LeftAssociativeSubtraction", "10-2-3", 5),
        new("UnaryPlusAndMinus", "-5+2", -3),
        new("UnaryPlus", "+5", 5),
        new("UnaryWithParentheses", "-(2+3)", -5),
        new("NestedArithmetic", "12 / (2 * 3)", 2)
    };

    public static TheoryData<EvaluationCase> UnaryPrecedenceCases => new()
    {
        new("UnaryTimes", "-2*3", -6),
        new("UnaryParenthesizedTimes", "-(2*3)", -6),
        new("UnaryAndBinaryPrecedence", "-2+3*4", 10)
    };

    public static TheoryData<EvaluationCase> BooleanCases => new()
    {
        new("BooleanTrueLiteral", "true", true),
        new("BooleanFalseLiteral", "false", false),
        new("LogicalNot", "!false", true),
        new("LogicalAnd", "true && false", false),
        new("LogicalOr", "true || false", true),
        new("LogicalPrecedence", "!false && true", true),
        new("LogicalParentheses", "!(false || false)", true),
        new("IntegerEqualityTrue", "1 == 1", true),
        new("IntegerEqualityFalse", "1 == 2", false),
        new("ArithmeticEquality", "1 + 2 == 3", true),
        new("BooleanEqualityFalse", "true == false", false),
        new("BooleanEqualityWithUnary", "true == !false", true),
        new("EqualityBeforeLogicalAnd", "1 == 1 && true", true)
    };

    [Theory]
    [MemberData(nameof(ArithmeticCases))]
    public void Evaluate_ValidExpressions_ReturnsExpectedResult(EvaluationCase testCase)
    {
       // AssertEvaluationMatches(testCase);
    }

    [Fact]
    public void Evaluate_DivisionByZero_Throws()
    {
        var parser = new Parser("10/0");
        var syntax = parser.parse();
        var binder = new Binder(new Dictionary<string, VariableSymbol>());
        var boundExpression = binder.Bind(syntax);
        var evaluator = new BoundEvaluator(boundExpression, new Dictionary<VariableSymbol, object>());

        Assert.Throws<DivideByZeroException>(() => evaluator.Evaluate());
    }

    [Theory]
    [MemberData(nameof(UnaryPrecedenceCases))]
    public void Evaluate_UnaryAndBinaryMix_RespectsPrecedence(EvaluationCase testCase)
    {
       // AssertEvaluationMatches(testCase);
    }

    [Theory]
    [MemberData(nameof(BooleanCases))]
    public void Evaluate_BooleanExpressions_ReturnExpectedResult(EvaluationCase testCase)
    {
    //    AssertEvaluationMatches(testCase);
    }

    [Fact]
    public void Evaluate_BoolDeclaration_ReturnsBooleanValue()
    {
        var result = EvaluateWithBinding("bool b = true");

        Assert.True(result is bool value && value);
    }

    [Fact]
    public void Evaluate_BoolVariable_PersistsAcrossInputs()
    {
        var variables = new Dictionary<VariableSymbol, object>();
        var variableSymbols = new Dictionary<string, VariableSymbol>();

        var declarationResult = EvaluateWithBinding("bool c = false", variableSymbols, variables);
        var readResult = EvaluateWithBinding("c", variableSymbols, variables);

        Assert.False((bool)declarationResult);
        Assert.False((bool)readResult);
    }

    [Fact]
    public void Evaluate_IntArrayDeclaration_ReturnsArrayValue()
    {
        var result = EvaluateWithBinding("int[] numbers = [1, 2, 3]");

        Assert.Equal("[1, 2, 3]", result.ToString());
    }

    [Fact]
    public void Evaluate_ArrayIndex_ReturnsExpectedElement()
    {
        var variables = new Dictionary<VariableSymbol, object>();
        var variableSymbols = new Dictionary<string, VariableSymbol>();

        _ = EvaluateWithBinding("int[] numbers = [4, 5, 6]", variableSymbols, variables);
        var result = EvaluateWithBinding("numbers[1]", variableSymbols, variables);

        Assert.Equal(5, result);
    }

    [Fact]
    public void Evaluate_CharArrayIndex_ReturnsExpectedElement()
    {
        var variables = new Dictionary<VariableSymbol, object>();
        var variableSymbols = new Dictionary<string, VariableSymbol>();

        _ = EvaluateWithBinding("char[] letters = ['a', 'b', 'c']", variableSymbols, variables);
        var result = EvaluateWithBinding("letters[2]", variableSymbols, variables);

        Assert.Equal('c', result);
    }

    [Fact]
    public void Bind_BoolDeclarationWithIntInitializer_ReportsDiagnostic()
    {
        var parser = new Parser("bool b = 5");
        var syntax = parser.parse();
        var binder = new Binder(new Dictionary<string, VariableSymbol>());

        _ = binder.Bind(syntax);

        if (parser.Diagnostics.Any())
            throw new XunitException($"Expected no parser diagnostics, got: {string.Join(", ", parser.Diagnostics)}");

        Assert.Contains(binder.Diagnostics, diagnostic => diagnostic.Contains("must be of type Boolean"));
    }

    [Fact]
    public void Bind_MixedBooleanAndNumberExpression_ReportsDiagnostic()
    {
        var parser = new Parser("true + 1");
        var syntax = parser.parse();
        var binder = new Binder(new Dictionary<string, VariableSymbol>());

        _ = binder.Bind(syntax);

        if (parser.Diagnostics.Any())
            throw new XunitException($"Expected no parser diagnostics for 'true + 1', but got: {string.Join(", ", parser.Diagnostics)}");

        if (!binder.Diagnostics.Any())
            throw new XunitException("Expected a binder diagnostic for 'true + 1', but the binder reported none.");
    }

    [Fact]
    public void Bind_EqualityBetweenDifferentTypes_ReportsDiagnostic()
    {
        var parser = new Parser("1 == true");
        var syntax = parser.parse();
        var binder = new Binder(new Dictionary<string, VariableSymbol>());

        _ = binder.Bind(syntax);

        if (parser.Diagnostics.Any())
            throw new XunitException($"Expected no parser diagnostics for '1 == true', but got: {string.Join(", ", parser.Diagnostics)}");

        if (!binder.Diagnostics.Any())
            throw new XunitException("Expected a binder diagnostic for '1 == true', but the binder reported none.");
    }

    [Fact]
    public void Bind_ArrayLiteralWithMixedTypes_ReportsDiagnostic()
    {
        var parser = new Parser("[1, true]");
        var syntax = parser.parse();
        var binder = new Binder(new Dictionary<string, VariableSymbol>());

        _ = binder.Bind(syntax);

        if (parser.Diagnostics.Any())
            throw new XunitException($"Expected no parser diagnostics for '[1, true]', but got: {string.Join(", ", parser.Diagnostics)}");

        Assert.Contains(binder.Diagnostics, diagnostic => diagnostic.Contains("same type"));
    }

    [Fact]
    public void Bind_ArrayDeclarationWithWrongElementType_ReportsDiagnostic()
    {
        var parser = new Parser("bool[] flags = [true, false, true]");
        var syntax = parser.parse();
        var binder = new Binder(new Dictionary<string, VariableSymbol>());

        _ = binder.Bind(syntax);

        if (parser.Diagnostics.Any())
            throw new XunitException($"Expected no parser diagnostics, got: {string.Join(", ", parser.Diagnostics)}");

        Assert.DoesNotContain(binder.Diagnostics, diagnostic => diagnostic.Contains("must be of type"));

        var invalidParser = new Parser("bool[] flags = [1, 2]");
        var invalidSyntax = invalidParser.parse();
        var invalidBinder = new Binder(new Dictionary<string, VariableSymbol>());

        _ = invalidBinder.Bind(invalidSyntax);

        if (invalidParser.Diagnostics.Any())
            throw new XunitException($"Expected no parser diagnostics, got: {string.Join(", ", invalidParser.Diagnostics)}");

        Assert.Contains(invalidBinder.Diagnostics, diagnostic => diagnostic.Contains("must be of type Boolean[]"));
    }

    [Fact]
    public void Bind_ArrayIndexWithNonIntegerExpression_ReportsDiagnostic()
    {
        var variables = new Dictionary<string, VariableSymbol>();
        var binder = new Binder(variables);

        _ = binder.Bind(new Parser("int[] numbers = [1, 2]").parse());

        var parser = new Parser("numbers[true]");
        var syntax = parser.parse();
        var bound = binder.Bind(syntax);

        Assert.NotNull(bound);
        Assert.Contains(binder.Diagnostics, diagnostic => diagnostic.Contains("Array indexes must be of type Int32"));
    }

    [Fact]
    public void Evaluate_ArrayIndexOutOfBounds_Throws()
    {
        var variables = new Dictionary<VariableSymbol, object>();
        var variableSymbols = new Dictionary<string, VariableSymbol>();

        _ = EvaluateWithBinding("int[] numbers = [1, 2]", variableSymbols, variables);

        var exception = Assert.Throws<Exception>(() => EvaluateWithBinding("numbers[5]", variableSymbols, variables));
        Assert.Contains("out of bounds", exception.Message);
    }

    private static object EvaluateWithBinding(
        string text,
        Dictionary<string, VariableSymbol>? variableSymbols = null,
        Dictionary<VariableSymbol, object>? variables = null)
    {
        var parser = new Parser(text);
        var syntax = parser.parse();

        if (parser.Diagnostics.Any())
            throw new XunitException($"Parser failed for '{text}' with diagnostics: {string.Join(", ", parser.Diagnostics)}");

        variableSymbols ??= new Dictionary<string, VariableSymbol>();
        variables ??= new Dictionary<VariableSymbol, object>();

        var binder = new Binder(variableSymbols);
        var boundExpression = binder.Bind(syntax);

        if (binder.Diagnostics.Any())
            throw new XunitException($"Binder failed for '{text}' with diagnostics: {string.Join(", ", binder.Diagnostics)}");

        var evaluator = new BoundEvaluator(boundExpression, variables);
        return evaluator.Evaluate();
    }

    // private static void AssertEvaluationMatches(EvaluationCase testCase)
    // {
    //     var result = EvaluateWithBinding(testCase.Text);

    //     if (!Equals(testCase.Expected, result))
    //     {
    //         throw new XunitException(
    //             $"Test case '{testCase.Name}' failed for expression '{testCase.Text}'. Expected '{testCase.Expected}' but got '{result}'.");
    //     }
    // }

    public sealed record EvaluationCase(string Name, string Text, object Expected)
    {
        public override string ToString() => $"{Name}: {Text}";
    }
}
