using Messaging;

namespace TurboPascal.Listeners;

/// <summary>
/// Writes parser diagnostics and summary information to the console.
/// </summary>
public sealed class ParserMessageListener : IMessageListener
{
    /// <inheritdoc />
    public void MessageReceived(Message message)
    {
        switch (message.Type)
        {
            case MessageType.SyntaxError:
                Console.WriteLine($"Syntax error: {message.Body}");
                break;

            case MessageType.ParserSummary:
                WriteSummary(message.Body);
                break;
        }
    }

    private static void WriteSummary(object body)
    {
        if (body is not object[] { Length: 3 } values ||
            values[0] is not int lineCount ||
            values[1] is not int errorCount ||
            values[2] is not float elapsedTime)
            return;

        Console.WriteLine($"{lineCount} source lines, {errorCount} syntax errors, {elapsedTime:F3} seconds.");
    }
}
