# AGENTS

## Project Snapshot
This repository is a small C# expression-language project that now has two layers:

- A syntax layer that lexes source text and builds an abstract syntax tree (AST).
- A binding layer that is starting to build a bound syntax tree from the AST.

The current focus is no longer just parsing. The parser already produces syntax nodes, and the next stage is binding those nodes into semantically meaningful bound nodes.

## Current Architecture

### Syntax Layer
Files in `syntaxanalysis/syntaxer/` are responsible for lexical analysis, parsing, and syntax tree nodes.

Current syntax support includes:

- Number literals
- Binary operators: `+`, `-`, `*`, `/`
- Unary operators: `+`, `-`
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
- `BoundBinaryexpression`
- `Boundunaryexpression`
- Operator kind enums for unary and binary operations
- `Binder` class that begins mapping syntax nodes into bound expressions

Important binding files:

- `syntaxanalysis/bindings/Binder.cs`
- `syntaxanalysis/bindings/bindings.cs`
- `syntaxanalysis/bindings/enums.cs`

## Current Status
The AST is in place and is more advanced than the original parser milestone. The parser currently handles precedence, unary expressions, and parentheses.

The bound syntax tree is in progress and is now part of the execution flow. The binder currently understands:

- Number expressions
- Binary expressions
- Unary expressions
- Parenthesized expressions

The current runtime path is:

1. Parse source text into syntax nodes
2. Bind syntax nodes into bound expressions
3. Evaluate the bound tree

Remaining work is mostly about expanding semantics, diagnostics, and future language features rather than just basic AST support.

## Why The Bound Tree Is Necessary
The syntax tree answers the question: "What did the user write?"

The bound tree answers the question: "What does that code mean after we understand it?"

That extra step is important because syntax nodes still contain parser-level details that are useful for reading source code but not ideal for semantic processing. A bound tree lets the compiler:

- Remove syntax-only structure that no longer matters semantically, such as parentheses wrapping an expression
- Convert raw tokens like `+`, `-`, `*`, `/` into explicit semantic operator kinds
- Attach meaning and type information to expressions
- Centralize semantic checks in one place instead of spreading them across the parser and evaluator
- Give later stages a cleaner representation for evaluation, optimization, or code generation

In this project, `Parenthessese` is a good example. Parentheses matter while parsing because they change precedence, but after binding the important part is just the inner expression. The bound tree keeps the meaning and drops the syntax noise.

Another example is operators. In the syntax tree, a binary expression stores an operator token. In the bound tree, that becomes a semantic operator kind such as `addition` or `subtraction`. That makes evaluation simpler and prepares the project for future checks like unsupported operators or type mismatches.

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
- If operator handling changes in the parser, verify the bound operator mapping at the same time
- Prefer documenting the real current state of the compiler pipeline over older README-era limitations

