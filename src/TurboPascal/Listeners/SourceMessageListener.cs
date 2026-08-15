using Messaging;

namespace TurboPascal.Listeners;

/// <summary>
/// Writes source lines received from the front end to the console.
/// </summary>
public sealed class SourceMessageListener : IMessageListener
{
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
}
