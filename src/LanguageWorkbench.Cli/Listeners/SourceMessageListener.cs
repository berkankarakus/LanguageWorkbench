using Messaging;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Writes source lines received from the front end to the console.
/// </summary>
/// <param name="flushTokens">Flushes token output before the next source line is written.</param>
/// <param name="useColor">Whether terminal colors should be used for source code.</param>
public sealed class SourceMessageListener(Action flushTokens, bool useColor = false) : IMessageListener
{
    private readonly PascalSourceHighlighter _highlighter = new(useColor);

    /// <summary>
    /// Called by the source whenever it produces a message.
    /// </summary>
    /// <param name="message">The source-line message to render.</param>
    /// <exception cref="ArgumentException">Thrown when the listener receives a non-source-line message.</exception>
    public void MessageReceived(Message message)
    {
        switch (message.Type)
        {
            case MessageType.SourceLine when message.Body is object?[] and [int lineNumber, string lineText]:
                flushTokens();
                _highlighter.WriteLine(lineNumber, lineText);
                break;

            default:
                throw new ArgumentException("Invalid type argument.");
        }
    }
}
