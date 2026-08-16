using Messaging;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Writes source lines received from the front end to the console.
/// </summary>
public sealed class SourceMessageListener : IMessageListener
{
    /// <summary>
    /// Called by the source whenever it produces a message.
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="ArgumentException"></exception>
    public void MessageReceived(Message message)
    {
        MessageType type = message.Type;
        object[] body = (object[])message.Body;

        switch (type)
        {
            case MessageType.SourceLine:
                int lineNumber = (int)body[0];
                string lineText = (string)body[1];

                Console.WriteLine($"{lineNumber,3:D3} {lineText}");
                break;

            default:
                throw new ArgumentException("Invalid type argument.");
        }
    }

    /*
    /// <inheritdoc />
    public void MessageReceived(Message message)
    {
        if (!TryGetSourceLine(message, out int lineNumber, out string line))
            return;

        Console.WriteLine($"{lineNumber,3:D3} {line}");
    }

    private static bool TryGetSourceLine(Message message, out int lineNumber, out string line)
    {
        lineNumber = default;
        line = string.Empty;

        if (message.Type != MessageType.SourceLine || message.Body is not object[] { Length: 2 } body)
            return false;

        if (body[0] is not int parsedLineNumber || body[1] is not string parsedLine)
            return false;

        lineNumber = parsedLineNumber;
        line = parsedLine;
        return true;
    }
    */
}
