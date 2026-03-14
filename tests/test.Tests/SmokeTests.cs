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
        var parser = new Parser(text);
        var syntax = parser.parse();

        Assert.Empty(parser.Diagnostics);

        var evaluator = new Evaluator(syntax);
        var result = evaluator.Evaluate();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Evaluate_DivisionByZero_Throws()
    {
        var parser = new Parser("10/0");
        var syntax = parser.parse();
        var evaluator = new Evaluator(syntax);

        Assert.Throws<DivideByZeroException>(() => evaluator.Evaluate());
    }

    [Theory]
    [InlineData("-2*3", -6)]
    [InlineData("-(2*3)", -6)]
    [InlineData("-2+3*4", 10)]
    public void Evaluate_UnaryAndBinaryMix_RespectsPrecedence(string text, int expected)
    {
        var parser = new Parser(text);
        var syntax = parser.parse();

        Assert.Empty(parser.Diagnostics);

        var evaluator = new Evaluator(syntax);
        var result = evaluator.Evaluate();

        Assert.Equal(expected, result);
    }
    
   
}