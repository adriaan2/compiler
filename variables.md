# Variable Support Plan

## Why This Is The Next Step

Right now the project can parse, bind, and evaluate literals, unary operators, binary operators, and parentheses. It cannot handle variables yet because:

- `Lexer` treats alphabetic text as either `true` / `false` or a bad token.
- `Parser` only builds literal, unary, binary, boolean, and parenthesized expressions.
- `Binder` has no symbol table and no bound nodes for variable reads or writes.
- `BoundEvaluator` has no variable storage.

That means the language can understand values, but not names for values.

## Recommended Direction

The cleanest first milestone is to add **assignment expressions** and **variable name expressions**.

Example target behavior:

```text
x = 10
x + 2
flag = true
!flag
x == 10
```

This fits the current architecture well because the project is still expression-based. It avoids introducing full statements too early.

Recommended rule for the first version:

- `name = expression` creates or updates a variable
- `name` reads a variable
- variables keep their value between REPL inputs in `Program.cs`

If you later want `let x = 10`, that is possible, but it pushes the project toward declarations/statements instead of only expressions. For now, plain assignment is the simpler path.

## What Needs To Change

### 1. Syntax Layer

Files to update:

- `syntaxanalysis/syntaxer/Lexer.cs`
- `syntaxanalysis/syntaxer/Parser.cs`
- `syntaxanalysis/syntaxer/Syntaxtoken.cs`
- `Program.cs`

New syntax files that will likely be needed:

- `syntaxanalysis/syntaxer/NameExpressionSyntax.cs`
- `syntaxanalysis/syntaxer/AssignmentExpressionSyntax.cs`

Work:

- Add `identifierToken` to `SyntaxKind`
- Add `equalsToken` for single `=`
- Keep `equalsEqualsToken` for `==`
- Change the lexer so alphabetic text becomes:
  - `trueKeyword` for `true`
  - `falseKeyword` for `false`
  - `identifierToken` for everything else
- Add a syntax node for reading a variable by name
- Add a syntax node for assignment expressions
- Update the parser so assignment is parsed before normal binary precedence
- Make assignment right-associative so `a = b = 10` can work later if desired
- Allow identifiers to appear as primary expressions
- Update `PrettyPrint` labels in `Program.cs` so the tree is still readable

Important note:

The current parser root type is `LiteralExpressionsyntax`. That is a strange name now, but variable expressions can still fit under it because they are still expressions. If the language later grows statements or declarations, this root type will probably need to be renamed or generalized.

### 2. Bound Layer

Files to update:

- `syntaxanalysis/bindings/Binder.cs`
- `syntaxanalysis/bindings/bindings.cs`
- `syntaxanalysis/bindings/enums.cs`

New binding types that will likely be needed:

- `VariableSymbol` or an equivalent symbol type
- `BoundVariableExpression`
- `BoundAssignmentExpression`

Work:

- Add new `Boundnodekind` values for variable read and assignment
- Add a symbol representation that stores at least:
  - variable name
  - variable type
- Give `Binder` access to a variable table
- When binding a name expression:
  - look up the variable
  - report a diagnostic if it does not exist
- When binding an assignment expression:
  - bind the right-hand side first
  - either create the variable or update an existing one
  - store the variable type from the bound expression
- Decide whether reassignment to a different type is allowed

Recommended first rule:

- Allow reassignment only when the type matches the existing variable type

That keeps the binder simple and preserves the project's current type-checking direction.

### 3. Evaluation Layer

Files to update:

- `syntaxanalysis/bindings/BoundEvaluator.cs`
- `Program.cs`

Work:

- Pass a variable storage dictionary into `BoundEvaluator`
- Evaluate variable reads by looking up the stored value
- Evaluate assignments by:
  - evaluating the right-hand side
  - storing the result
  - returning the stored value
- Move the variable storage dictionary outside the REPL loop in `Program.cs` so values persist between lines

This keeps the runtime path aligned with `AGENTS.md`:

1. parse source text
2. bind syntax into bound nodes
3. evaluate the bound tree

### 4. Diagnostics

Diagnostics should stay centered in the binder instead of being pushed into the parser or evaluator.

New diagnostics to add:

- undefined variable name
- assignment to an existing variable with the wrong type
- any malformed assignment shape the parser allows but binding rejects

The parser should only care about structure. The binder should decide what the variable use means.

### 5. Tests

Files to update:

- `tests/test.Tests/SmokeTests.cs`

Tests to add:

- assignment returns the assigned value
- assigned integer variable can be reused
- assigned boolean variable can be reused
- variables work inside arithmetic expressions
- variables work inside logical expressions
- equality works with variable operands
- undefined variable reports a binder diagnostic
- reassigning with a different type reports a binder diagnostic if type changes are rejected
- assignment respects precedence, for example `a = 1 + 2 * 3`

Because variables need persistent state, some tests should create a shared variable dictionary across multiple evaluations.

## Suggested Implementation Order

1. Add `identifierToken` and `equalsToken` in `SyntaxKind`
2. Teach `Lexer` to recognize identifiers and single `=`
3. Add name-expression and assignment-expression syntax nodes
4. Update `Parser` to parse assignments and identifier expressions
5. Add bound node kinds and bound expression types for variables
6. Add a variable symbol table to `Binder`
7. Update `BoundEvaluator` to read and write variables
8. Update `Program.cs` so variables persist across REPL inputs
9. Add smoke tests for happy paths and diagnostics

## Design Choice To Confirm Later

There is one important fork in the road:

- **Option A:** support `x = 10` first
- **Option B:** support `let x = 10` first

Recommendation:

- Start with **Option A**

Reason:

- it fits the current expression-only design
- it needs fewer syntax categories
- it lets the binder and evaluator grow without introducing statements yet

If `let` is added later, the project may need a separate declaration or statement layer.

## Expected End State

After this work, the project should be able to:

- lex identifiers
- parse variable reads and assignments
- bind them into semantic nodes with type information
- evaluate them through the bound tree
- preserve variable values across REPL input lines

That would make variables the next real semantic feature built on top of the existing AST and bound-tree pipeline instead of bypassing it.
