# AGENTS

## Project Snapshot
This repository is a small C# expression-language project that now has two layers:

- A syntax layer that lexes source text and builds an abstract syntax tree (AST).
- A binding layer that lowers the AST into a semantically checked bound tree.

The current focus is no longer just parsing. The parser already produces syntax nodes, and the compiler pipeline now supports binding and evaluating typed expressions and variables.

## Current Architecture

### Syntax Layer
Files in `syntaxanalysis/syntaxer/` are responsible for lexical analysis, parsing, and syntax tree nodes.

Current syntax support includes:

- Number literals
- Boolean literals: `true`, `false`
- Char literals
- Variable declarations for `int`, `bool`, and `char`
- Variable references and assignment expressions
- Binary operators: `+`, `-`, `*`, `/`, `&&`, `||`, `==`
- Unary operators: `+`, `-`, `!`
- Parenthesized expressions

Important syntax files:

- `syntaxanalysis/syntaxer/Lexer.cs`
- `syntaxanalysis/syntaxer/Parser.cs`
- `syntaxanalysis/syntaxer/Syntaxtoken.cs`
- `syntaxanalysis/syntaxer/UnaryExpressionSyntax.cs`
- `syntaxanalysis/syntaxer/Parenthessese.cs`

The syntax tree root type used by the parser is `LiteralExpressionsyntax`.

### Bound Layer
Files in `syntaxanalysis/bindings/` are responsible for lowering syntax nodes into bound nodes.

Current bound-node work includes:

- `BoundNumberexpression`
- `BoundBooleanexpression`
- `Boundcharexpression`
- `BoundBinaryexpression`
- `Boundunaryexpression`
- `BoundVariableexpression`
- `BoundAssignmentexpression`
- `BoundVariabledeclarationexpression`
- `VariableSymbol`
- Operator kind enums for unary and binary operations
- `Binder` class that maps syntax nodes into bound expressions and enforces basic type rules
- `BoundEvaluator` class that evaluates bound expressions against a variable store

Important binding files:

- `syntaxanalysis/bindings/Binder.cs`
- `syntaxanalysis/bindings/bindings.cs`
- `syntaxanalysis/bindings/enums.cs`

## Current Status
The AST is in place and is more advanced than the original parser milestone. The parser currently handles arithmetic, boolean literals, char literals, variable declarations for `int`, `bool`, and `char`, name lookup, assignment expressions, logical operators, equality, precedence, unary expressions, and parentheses.

The bound syntax tree is in progress and is now part of the execution flow. The binder currently understands:

- Number expressions
- Boolean expressions
- Char expressions
- Binary expressions
- Unary expressions
- Parenthesized expressions
- Variable declarations
- Variable references
- Assignment expressions

The binder also performs basic semantic validation, including:

- Arithmetic operators only for `int`
- Logical operators only for `bool`
- Equality checks for matching operand types such as `int == int`, `bool == bool`, and `char == char`
- Variable declarations must use one of the supported declared types: `int`, `bool`, or `char`
- Variable initializers and assignments must match the declared variable type
- Variable references and assignments require the variable to already exist

The current runtime path is:

1. Parse source text into syntax nodes
2. Bind syntax nodes into bound expressions
3. Evaluate the bound tree, including variable declarations, reads, assignments, and typed literal values

Remaining work is mostly about expanding semantics, improving diagnostics, and adding future language features rather than just basic AST support.


## Preferred Direction
When continuing work, prefer this pipeline:

1. `Lexer` produces tokens
2. `Parser` produces AST / syntax nodes
3. `Binder` produces bound expressions
4. Evaluation operates on bound nodes, not raw syntax nodes

## Notes For Future Edits

- Keep syntax concerns in `syntaxanalysis/syntaxer/`
- Keep semantic/binding concerns in `syntaxanalysis/bindings/`
- If a new syntax node is added, update the binder so the AST and bound tree stay in sync
- If a new declared type is added, update the lexer/parser keywords, binder type mapping, operator validation, and evaluator behavior together
- If variable syntax changes, verify declaration, lookup, assignment, and runtime storage all stay aligned
- If operator handling changes in the parser, verify the bound operator mapping at the same time
- Prefer documenting the real current state of the compiler pipeline over older README-era limitations
