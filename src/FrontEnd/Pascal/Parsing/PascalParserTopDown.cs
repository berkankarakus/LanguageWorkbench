using System.Diagnostics;
using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Parsing;
using FrontEnd.Scanning;
using FrontEnd.Tokens;
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

                if (tokenType is null || tokenType == PascalTokenType.Error)
                {
                    if (token.Value is PascalErrorCode errorCode)
                        ErrorHandler.Flag(token, errorCode, this);
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
