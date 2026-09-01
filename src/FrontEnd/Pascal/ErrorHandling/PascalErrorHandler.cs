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
        parser.SendMessage(new(
            MessageType.SyntaxError,
            new object?[]
            {
                token.LineNumber,
                token.Position,
                token.Text,
                errorCode.Message
            })
        );

        ++ErrorCount;
        throw new PascalSyntaxException();
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

/// <summary>
/// Signals that parsing must stop after a reported Pascal syntax error.
/// </summary>
internal sealed class PascalSyntaxException : Exception;
