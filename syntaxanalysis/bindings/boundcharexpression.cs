using bindings;

internal sealed class Boundcharexpression : Boundexpression
{
    public Boundcharexpression(char value)
    {
        Value=value;
    }
    public override Type Type => typeof(char);
    public char Value;

    internal override Boundnodekind Boundnodekind => Boundnodekind.charexpression;
}