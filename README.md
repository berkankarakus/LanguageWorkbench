# LanguageWorkbench

LanguageWorkbench is a .NET 10 learning project for building a language-processing pipeline. It currently scans Pascal source, emits token and diagnostic messages, and provides placeholder compile and execute back ends.

## Status

The Pascal lexical scanner is implemented for reserved words, identifiers, integer and real literals, string literals, comments, and Pascal special symbols. The parser currently walks the token stream and reports scanner errors; it does not yet build a complete syntax tree or generate executable instructions.

## Projects

| Project | Responsibility |
| --- | --- |
| `src/LanguageWorkbench.Cli` | Command-line entry point and console listeners. |
| `src/FrontEnd` | Source reader, scanner, tokens, parser abstractions, and Pascal front end. |
| `src/Intermediate` | Intermediate-code and symbol-table contracts. |
| `src/BackEnd` | Compile and execute back-end placeholders. |
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
dotnet run --project src/LanguageWorkbench.Cli -- compile src/LanguageWorkbench.Cli/hello.pas
dotnet run --project src/LanguageWorkbench.Cli -- execute src/LanguageWorkbench.Cli/hello.pas
```

Use `-x` to print the symbol-table cross-reference. Set `LANGUAGE_WORKBENCH_SHOW_TOKENS=true` (or `1`) to include token output.

## Output

For each source line, the CLI prints recognized tokens and their position. Scanner diagnostics show a caret under the source character that caused the error, followed by a human-readable error message.

## Architecture

```text
SourceReader -> PascalScanner -> PascalParser -> BackendProcessor
                   |                    |
                   +-> Token messages   +-> Parser summary / diagnostics
```

`SourceReader`, `Parser`, and `BackendProcessor` publish `Messaging.Message` instances to listeners. The CLI listeners format these events without coupling the front end to console output.

## Development notes

- Pascal keywords are matched case-insensitively.
- The scanner represents Pascal `REAL` literals as .NET `double` values.
- The included `hello.pas` file is a scanner smoke-test input, not a full compiler conformance test.
