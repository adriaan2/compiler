public class Program
{
    public static void Main()
    {
        Console.ForegroundColor=ConsoleColor.White;
        while (true)
        {
            
        
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
            return;

        Parser parser = new(line);
        var expression = parser.parse();
        PrettyPrint(expression);
        if (parser.Diagnostics.Any())
        {
            Console.ForegroundColor=ConsoleColor.Red;
            foreach (var item in parser.Diagnostics)
            {
                System.Console.WriteLine($"badtoken at {parser.Position}");
            }
        }  
        else
        {
            Evaluator evaluator=new(expression);
            var result=evaluator.Evaluate();
            System.Console.WriteLine(result);
        }
    }}

    public static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
    {
        var marker = indent == "" ? "" : (isLast ? "└──" : "├──");
        Console.WriteLine($"{indent}{marker}{GetNodeLabel(node)}");

        indent += indent == "" ? "    " : (isLast ? "    " : "│   ");

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
            SyntaxKind.numberexpression => "NumberExpression",
            SyntaxKind.numberToken => $"NumberToken {(node as Syntaxtoken)?.Value}",
            SyntaxKind.plusToken => "PlusToken",
            SyntaxKind.minusToken => "MinusToken",
            SyntaxKind.timestoken => "StarToken",
            SyntaxKind.slashtoken => "SlashToken",
            _ => node.Kind.ToString()
        };
    }
}
