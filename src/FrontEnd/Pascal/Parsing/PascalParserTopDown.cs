using System.Diagnostics;
using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Parsing;
using FrontEnd.Scanning;
using FrontEnd.Tokens;
using Intermediate.Symbols;
using Messaging;

namespace FrontEnd.Pascal.Parsing;

/// <summary>
/// The top-down Pascal parser.
/// </summary>
/// <param name="scanner">The scanner that supplies Pascal tokens.</param>
public class PascalParserTopDown(Scanner scanner) : Parser(scanner)
{
    private static readonly PascalErrorHandler ErrorHandler = new();

    /// <summary>
    /// Parse a Pascal source program and generate the symbol table and the intermediate code.
    /// </summary>
    public override void Parse()
    {
        try
        {
            Token token;
            long startTime = Stopwatch.GetTimestamp();

            do
            {
                token = NextToken();
                ITokenType? tokenType = token.Type;

                try
                {
                    // cross-reference only the identifiers.
                    if (tokenType == PascalTokenType.Identifier)
                    {
                        string? name = token.Text?.ToLowerInvariant();

                        // If it's not already in the symbol table, create & enter a new entry for the identifier.
                        if (!string.IsNullOrEmpty(name))
                        {
                            SymbolTableEntry entry =
                                SymbolTableStack.Lookup(name)
                                ?? SymbolTableStack.EnterLocal(name);

                            entry.AppendLineNumber(token.LineNumber);
                        }

                        else if (tokenType == PascalTokenType.Error && token.Value != null)
                            ErrorHandler.Flag(token, (PascalErrorCode)token.Value, parser: this);
                    }

                    double tokenElapsedTime = Stopwatch.GetElapsedTime(startTime).TotalSeconds;
                    SendMessage(new(
                        MessageType.ParserSummary,
                        new object?[]
                        {
                            token.LineNumber,
                            GetErrorCount(),
                            tokenElapsedTime
                        })
                    );
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    PascalErrorHandler.AbortTranslation(PascalErrorCode.IoError, parser: this);
                }

                if (tokenType is null || tokenType == PascalTokenType.Error)
                {
                    if (token.Value is PascalErrorCode errorCode)
                        ErrorHandler.Flag(token, errorCode, parser: this);
                }

                else
                {
                    SendMessage(new(
                        MessageType.Token,
                        new[]
                        {
                            token.LineNumber,
                            token.Position,
                            tokenType,
                            token.Text,
                            token.Value
                        }
                    ));
                }
            } while (token is not EofToken);

            // Send the parser summary message.
            float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;
            SendMessage(new(
                MessageType.ParserSummary,
                new object?[]
                {
                    token.LineNumber,
                    GetErrorCount(),
                    elapsedTime
                })
            );
        }

        catch (IOException)
        {
            PascalErrorHandler.AbortTranslation(PascalErrorCode.IoError, this);
        }
    }

    /// <summary>
    /// Returns the number of syntax errors found while parsing.
    /// </summary>
    /// <returns>The number of reported syntax errors.</returns>
    public override int GetErrorCount()
    {
        return ErrorHandler.ErrorCount;
    }
}
