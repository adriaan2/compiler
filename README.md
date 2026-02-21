# Mini Expression Parser (C#)

## Overview
This project is a small expression parser in C# that tokenizes input text and builds a syntax tree (AST) for simple arithmetic expressions.

It currently supports numeric literals and left-associative binary `+` / `-` parsing.

## File Structure
- `Program.cs`: Entry point, parser invocation, and AST pretty-printer helper.
- `syntaxer/Lexer.cs`: Converts input text into `Syntaxtoken` instances.
- `syntaxer/Parser.cs`: Consumes tokens and builds expression syntax nodes.
- `syntaxer/Syntaxtoken.cs`: Defines token kinds and AST node types.

## Supported Tokens (`SyntaxKind`)
- `numberToken`
- `plusToken` (`+`)
- `minusToken` (`-`)
- `slashtoken` (`/`)
- `timestoken` (`*`)
- `openparen` (`(`)
- `closedparen` (`)`)
- `whitespaceToken`
- `badtoken`
- `endoffiletoken`
- expression node kinds: `numberexpression`, `binaryexpression`

## Effective Grammar (Current Parser)
```text
expression := primary (("+" | "-") primary)*
primary    := number
number     := digit+
```

Note: The lexer emits `/`, `*`, and parenthesis tokens, but the parser does not yet process them.

## How It Works
1. `Program.Main` creates a `Parser` with a hardcoded input string.
2. `Parser` runs `Lexer.Nextoken()` repeatedly until `endoffiletoken`.
3. Whitespace and bad tokens are filtered out.
4. `Parser.parse()` builds a left-associative binary tree for `+` and `-`.
5. The resulting expression node is printed.

## Core Types
- `SyntaxNode`: Base type for syntax tree elements.
- `ExpressionSyntax`: Base type for expression nodes.
- `Syntaxtoken`: Token node with `Kind`, `POsition`, `Text`, `Value`.
- `numberSyntax`: Numeric expression node.
- `BynarySyntax`: Binary expression node with `Left`, `OperatorToken`, `Right`.

## Example
```csharp
var parser = new Parser("12+3-4");
ExpressionSyntax tree = parser.parse();
```

AST shape:
```text
((12 + 3) - 4)
```

## Current Limitations
- Parser only handles numbers with `+` and `-`.
- No unary operators.
- No parenthesis parsing.
- Minimal diagnostics/error reporting.
- Several identifiers are misspelled but consistently referenced:
  - `Nextoken`
  - `BynarySyntax`
  - `POsition`

## Run
From the project root:
```bash
dotnet run
```

The current `Program.cs` uses a hardcoded test string.
