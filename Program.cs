using bindings;
using System.Collections.Generic;
using syntaxer;

public class Program
{
    public static void Main()
    {
        Console.ForegroundColor=ConsoleColor.White;
        var variables = new Dictionary<VariableSymbol, object>();
        var variableSymbols = new Dictionary<string, VariableSymbol>();
        while (true)
        {
            
        
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
            return;

       syntaxer.Parser parser = new(line);
        var expression = parser.parse();
        PrettyPrint(expression);
        if (parser.Diagnostics.Any())
        {
            Console.ForegroundColor=ConsoleColor.Red;
            foreach (var item in parser.Diagnostics)
            {
                System.Console.WriteLine(item);
            }
        }  
        else
        {
            var binder = new Binder(variableSymbols);
            var boundExpression = binder.Bind(expression);

            if (binder.Diagnostics.Any())
            {
                Console.ForegroundColor=ConsoleColor.Red;
                foreach (var item in binder.Diagnostics)
                {
                    System.Console.WriteLine(item);
                }

                continue;
            }

            BoundEvaluator evaluator=new(boundExpression, variables);
            var result=evaluator.Evaluate();
            System.Console.WriteLine(result);
        }
    }}

    public static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
    {
                var marker = indent == "" ? "" : (isLast ? "+--" : "+--");
        Console.WriteLine($"{indent}{marker}{GetNodeLabel(node)}");

        indent += indent == "" ? "    " : (isLast ? "    " : "¦   ");

        var children = node.getchildren().ToArray();
        for (int i = 0; i < children.Length; i++)
        {
            PrettyPrint(children[i], indent, i == children.Length - 1);
        }
    }

    private static string GetNodeLabel(SyntaxNode node)
    {
        return node.Kind switch
        {
            SyntaxKind.binaryexpression => "BinaryExpression",
            SyntaxKind.unaryexpression => "UnaryExpression",
            SyntaxKind.numberexpression => "NumberExpression",
            SyntaxKind.booleanexpression => "BooleanExpression",
            SyntaxKind.arrayexpression => "ArrayExpression",
            SyntaxKind.arrayindexexpression => "ArrayIndexExpression",
            SyntaxKind.typeclause => "TypeClause",
            SyntaxKind.nameexpression => "NameExpression",
            SyntaxKind.assignmentexpression => "AssignmentExpression",
            SyntaxKind.variabledeclarationexpression => "VariableDeclarationExpression",
            SyntaxKind.numberToken => $"NumberToken {(node as Syntaxtoken)?.Value}",
            SyntaxKind.openbracket => "OpenBracketToken",
            SyntaxKind.closebracket => "CloseBracketToken",
            SyntaxKind.commaToken => "CommaToken",
            SyntaxKind.identifierToken => $"IdentifierToken {(node as Syntaxtoken)?.Text}",
            SyntaxKind.trueKeyword => "TrueKeyword",
            SyntaxKind.falseKeyword => "FalseKeyword",
            SyntaxKind.intKeyword => "IntKeyword",
            SyntaxKind.boolKeyword => "BoolKeyword",
            SyntaxKind.charkeyword => "CharKeyword",
            SyntaxKind.equalsToken => "EqualsToken",
            SyntaxKind.equalsEqualsToken => "EqualsEqualsToken",
            SyntaxKind.plusToken => "PlusToken",
            SyntaxKind.minusToken => "MinusToken",
            SyntaxKind.timestoken => "StarToken",
            SyntaxKind.slashtoken => "SlashToken",
            SyntaxKind.bangToken => "BangToken",
            SyntaxKind.ampersandAmpersandToken => "AmpersandAmpersandToken",
            SyntaxKind.pipePipeToken => "PipePipeToken",
            _ => node.Kind.ToString()
        };
    }
}

