using FrontEnd.Parsing;
using FrontEnd.Tokens;
using Messaging;

namespace FrontEnd.Pascal.ErrorHandling;

/// <summary>
/// Error handler for Pascal syntax errors.
/// </summary>
public class PascalErrorHandler
{
    private const int MaxErrors = 25;
    public int ErrorCount { get; private set; }

    /// <summary>
    /// Flag an error in the source file.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="errorCode"></param>
    /// <param name="parser"></param>
    public void Flag(Token token, PascalErrorCode errorCode, Parser parser)
    {
        parser.SendMessage(new(
            MessageType.SyntaxError,
            new object?[]
            {
                token.LineNumber,
                token.Position,
                token.Text,
                errorCode.ToString()
            })
        );

        if (++ErrorCount > MaxErrors)
            AbortTranslation(PascalErrorCode.TooManyErrors, parser);
    }

    public void AbortTranslation(PascalErrorCode errorCode, Parser parser)
    {
        string fatalText = "FATAL ERROR: " + errorCode.ToString();
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

        Environment.Exit(errorCode.GetStatus());
    }
}
