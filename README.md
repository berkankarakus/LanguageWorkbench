# LanguageWorkbench

LanguageWorkbench is a .NET 10 learning project for building a language-processing pipeline. It currently scans, parses, and interprets a growing Pascal subset while exposing the intermediate code and symbol-table data used between stages.

## Status

The Pascal lexical scanner supports reserved words, identifiers, integer and real literals, string literals, comments, and Pascal special symbols. The active top-down parser builds intermediate code for compound statements, assignments, expressions, `IF`, `REPEAT`, `WHILE`, `FOR`, and `CASE` statements. The interpreter evaluates expressions and executes those control-flow constructs.

The bottom-up parser remains experimental and is not selected by the CLI. Instruction generation for `compile` is intentionally still a placeholder.

## Projects

| Project | Responsibility |
| --- | --- |
| `src/LanguageWorkbench.Cli` | Command-line entry point and console listeners. |
| `src/FrontEnd` | Source reader, scanner, tokens, parser abstractions, and Pascal front end. |
| `src/Intermediate` | Intermediate-code and symbol-table contracts. |
| `src/BackEnd` | Intermediate-code interpreter and placeholder instruction generator. |
| `src/Messaging` | In-process producer/listener messaging infrastructure. |

## Requirements

- .NET SDK 10.0 or later

## Build

```bash
dotnet build LanguageWorkbench.sln
```

## Run

The CLI accepts `compile` and `execute` operations followed by a Pascal source-file path.

```bash
dotnet run --project src/LanguageWorkbench.Cli -- compile -i src/LanguageWorkbench.Cli/repeat.pas
dotnet run --project src/LanguageWorkbench.Cli -- execute src/LanguageWorkbench.Cli/repeat.pas
```

Flags can be combined:

- `-i`: print intermediate code.
- `-x`: print the symbol-table cross-reference.

Set `LANGUAGE_WORKBENCH_SHOW_TOKENS=true` (or `1`) to include token output. Set `LANGUAGE_WORKBENCH_OUTPUT_PRECISION` to an integer from `1` through `17` to choose the significant-digit precision used for floating-point interpreter output.

## Output

The CLI writes colored source, diagnostics, intermediate code, and interpreter output when attached to an interactive terminal. Redirected output remains plain text. Scanner diagnostics show a caret under the source character that caused the error, followed by a human-readable error message.

## Architecture

```text
SourceReader -> PascalScanner -> PascalParserTopDown -> Intermediate Code -> BackendProcessor
                   |                    |                     |
                   +-> Token messages   +-> Diagnostics         +-> Interpreter or code generator
```

`SourceReader`, `Parser`, and `BackendProcessor` publish `Messaging.Message` instances to listeners. The CLI listeners format these events without coupling the front end to console output.

## Development notes

- Pascal keywords are matched case-insensitively.
- The scanner represents Pascal `REAL` literals as .NET `double` values.
- The included Pascal samples are smoke-test inputs, not a full Pascal conformance suite.
- `compile` currently reports instruction-generation metadata but does not emit executable instructions.
