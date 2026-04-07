
namespace syntaxer;

internal sealed class Charsyntax : LiteralExpressionsyntax
{
    public Charsyntax(Syntaxtoken openQuote, Syntaxtoken characterValue, Syntaxtoken closeQuote)
    {
        OpenQuote = openQuote;
        CharacterValue = characterValue;
        CloseQuote = closeQuote;
    }

    public Syntaxtoken OpenQuote { get; }
    public Syntaxtoken CharacterValue { get; }
    public Syntaxtoken CloseQuote { get; }

    public override SyntaxKind Kind => SyntaxKind.charvaltoken;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return OpenQuote;
        yield return CharacterValue;
        yield return CloseQuote;
    }
}
