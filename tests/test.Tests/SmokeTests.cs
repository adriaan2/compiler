using Xunit;
using syntaxer;
namespace test.Tests;

public class SmokeTests
{
    [Theory]
    [InlineData("1+2", 3)]
    [InlineData("7-4", 3)]
    [InlineData("3*5", 15)]
    [InlineData("8/2", 4)]
    [InlineData("1+2*3", 7)]
    [InlineData("(1+2)*3", 9)]
    [InlineData("10-2-3", 5)]
    [InlineData("-5+2", -3)]
    [InlineData("+5", 5)]
    [InlineData("-(2+3)", -5)]
    [InlineData("12 / (2 * 3)", 2)]
    public void Evaluate_ValidExpressions_ReturnsExpectedResult(string text, int expected)
    {
        var result = EvaluateWithBinding(text);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Evaluate_DivisionByZero_Throws()
    {
        var parser = new Parser("10/0");
        var syntax = parser.parse();
        var binder = new Binder();
        var boundExpression = binder.Bind(syntax);
        var evaluator = new BoundEvaluator(boundExpression);

        Assert.Throws<DivideByZeroException>(() => evaluator.Evaluate());
    }

    [Theory]
    [InlineData("-2*3", -6)]
    [InlineData("-(2*3)", -6)]
    [InlineData("-2+3*4", 10)]
    public void Evaluate_UnaryAndBinaryMix_RespectsPrecedence(string text, int expected)
    {
        var result = EvaluateWithBinding(text);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("!false", true)]
    [InlineData("true && false", false)]
    [InlineData("true || false", true)]
    [InlineData("!false && true", true)]
    [InlineData("!(false || false)", true)]
    [InlineData("1 == 1", true)]
    [InlineData("1 == 2", false)]
    [InlineData("1 + 2 == 3", true)]
    [InlineData("true == false", false)]
    [InlineData("true == !false", true)]
    [InlineData("1 == 1 && true", true)]
    public void Evaluate_BooleanExpressions_ReturnExpectedResult(string text, bool expected)
    {
        var result = EvaluateWithBinding(text);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Bind_MixedBooleanAndNumberExpression_ReportsDiagnostic()
    {
        var parser = new Parser("true + 1");
        var syntax = parser.parse();
        var binder = new Binder();

        _ = binder.Bind(syntax);

        Assert.Empty(parser.Diagnostics);
        Assert.NotEmpty(binder.Diagnostics);
    }

    [Fact]
    public void Bind_EqualityBetweenDifferentTypes_ReportsDiagnostic()
    {
        var parser = new Parser("1 == true");
        var syntax = parser.parse();
        var binder = new Binder();

        _ = binder.Bind(syntax);

        Assert.Empty(parser.Diagnostics);
        Assert.NotEmpty(binder.Diagnostics);
    }

    private static object EvaluateWithBinding(string text)
    {
        var parser = new Parser(text);
        var syntax = parser.parse();

        Assert.Empty(parser.Diagnostics);

        var binder = new Binder();
        var boundExpression = binder.Bind(syntax);

        Assert.Empty(binder.Diagnostics);

        var evaluator = new BoundEvaluator(boundExpression);
        return evaluator.Evaluate();
    }
}
