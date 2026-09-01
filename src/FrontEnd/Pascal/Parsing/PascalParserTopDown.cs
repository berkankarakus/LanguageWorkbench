using System.Diagnostics;
using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Parsing;
using FrontEnd.Scanning;
using FrontEnd.Tokens;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Interfaces;
using Messaging;

namespace FrontEnd.Pascal.Parsing;

/// <summary>
/// The top-down Pascal parser.
/// </summary>
/// <param name="scanner">The scanner that supplies Pascal tokens.</param>
public class PascalParserTopDown(Scanner scanner) : Parser(scanner)
{
    protected static readonly PascalErrorHandler ErrorHandler = new();

    /// <summary>
    /// Creates a parser that uses the scanner of an existing parser.
    /// </summary>
    /// <param name="parent">The parser whose scanner will be reused.</param>
    public PascalParserTopDown(PascalParserTopDown parent) : this(parent.Scanner) { }

    /// <summary>
    /// Processes Pascal tokens and publishes their diagnostics and parsing summary.
    /// </summary>
    public void ProcessTokens()
    {
        long startTime = Stopwatch.GetTimestamp();

        try
        {
            Token token;

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
                            ISymbolTableEntry entry =
                                SymbolTableStack.Lookup(name)
                                ?? SymbolTableStack.EnterLocal(name);

                            entry.AppendLineNumber(token.LineNumber);
                        }

                        else if (tokenType == PascalTokenType.Error && token.Value != null)
                            ErrorHandler.Flag(token, (PascalErrorCode)token.Value, parser: this);
                    }

                    float tokenElapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;
                    SendMessage(new(
                        MessageType.ParserSummary,
                        new object?[]
                        {
                            token.LineNumber,
                            ErrorCount,
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
                    ErrorCount,
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
    /// Parses a Pascal source program and generates its intermediate code.
    /// </summary>
    public override void Parse()
    {
        long startTime = Stopwatch.GetTimestamp();

        try
        {
            Token? token = NextToken();
            IIntermediateCodeNode? rootNode = null;

            // Look for the BEGIN token to parse a compund statement.
            if (token.Type != PascalTokenType.Begin)
                ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, parser: this);

            else
            {
                StatementParser statementParser = new(this);
                rootNode = statementParser.Parse(token);
                token = CurrentToken();
            }

            // Look for the final period.
            if (token is not null && token.Type != PascalTokenType.Dot)
                ErrorHandler.Flag(token, PascalErrorCode.MissingPeriod, parser: this);

            token = CurrentToken();

            // Set the parse tree root node.
            if (rootNode is not null)
                Code.Root = rootNode;

            // Send the parser summary message.
            float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;

            if (token is not null)
            {
                SendMessage(new Message(
                    MessageType.ParserSummary,
                    body: new[]
                    {
                        token.LineNumber,
                        ErrorCount,
                        elapsedTime
                    })
                );
            }
        }

        catch (PascalSyntaxException)
        {
            // The syntax error was already reported by PascalErrorHandler.
        }

        catch (Exception)
        {
            PascalErrorHandler.AbortTranslation(PascalErrorCode.IoError, parser: this);
        }
    }

    /// <summary>
    /// Gets the number of syntax errors found while parsing.
    /// </summary>
    public override int ErrorCount => ErrorHandler.ErrorCount;
}
