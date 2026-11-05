using FrontEnd.Parsing;
using FrontEnd.Tokens;
using Messaging;

namespace FrontEnd.Pascal.ErrorHandling;

/// <summary>
/// Error handler for Pascal syntax errors.
/// </summary>
public class PascalErrorHandler
{
    /// <summary>
    /// Gets the number of syntax errors reported by this handler.
    /// </summary>
    public int ErrorCount { get; private set; }

    /// <summary>
    /// Flag an error in the source file.
    /// </summary>
    /// <param name="token">The token associated with the error.</param>
    /// <param name="errorCode">The error to report.</param>
    /// <param name="parser">The parser that publishes the diagnostic.</param>
    public void Flag(Token token, PascalErrorCode errorCode, Parser parser)
    {
        Flag(token, errorCode.Message, parser);
    }

    /// <summary>
    /// Reports a syntax error with a context-specific message.
    /// </summary>
    /// <param name="token">The token associated with the error.</param>
    /// <param name="message">The diagnostic message to report.</param>
    /// <param name="parser">The parser that publishes the diagnostic.</param>
    public void Flag(Token token, string message, Parser parser)
    {
        ArgumentNullException.ThrowIfNull(message);

        parser.SendMessage(new(
            MessageType.SyntaxError,
            new object?[]
            {
                token.LineNumber,
                token.Position,
                token.Text,
                message
            })
        );

        ++ErrorCount;
    }

    /// <summary>
    /// Reports a fatal error and terminates the translation process.
    /// </summary>
    /// <param name="errorCode">The fatal error to report.</param>
    /// <param name="parser">The parser that publishes the diagnostic.</param>
    public static void AbortTranslation(PascalErrorCode errorCode, Parser parser)
    {
        string fatalText = "FATAL ERROR: " + errorCode.Message;
        parser.SendMessage(new(
            MessageType.SyntaxError,
            new object?[]
            {
                0,
                0,
                "",
                fatalText
            })
        );

        Environment.Exit(errorCode.Status);
    }
}
