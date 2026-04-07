
namespace bindings{
internal enum Boundnodekind
    {
        charexpression,
        Unaryexpression,
        Numberexpression,
        Binaryexpression,
        Booleanexpression,
        Variableexpression,
        Assignmentexpression,
        Variabledeclarationexpression
    }
internal enum Boundunaryoperatorkind
    {
        Identity,
        Negation,
        LogicalNegation
    }
 internal enum BoundBinaryoperatorkind
    {
        addition,
        subtraction,
        times,
        division,
        Equals,
        LogicalAnd,
        LogicalOr
    }
    
    }
    
