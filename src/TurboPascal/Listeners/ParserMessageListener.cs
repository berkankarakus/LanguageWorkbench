using Messaging;

namespace TurboPascal.Listeners;

/// <summary>
/// Writes parser diagnostics and summary information to the console.
/// </summary>
public sealed class ParserMessageListener : IMessageListener
{
    /// <summary>
    /// Called by the parser whenever it produces a message.
    /// </summary>
    /// <param name="message"></param>
    public void MessageReceived(Message message)
    {
        MessageType type = message.Type;

        switch (type)
        {
            case MessageType.ParserSummary:
                object[] body = (object[])message.Body;
                int statementCount = (int)body[0];
                int syntaxErrorCount = (int)body[1];
                float elapsedTime = (float)body[2];

                Console.WriteLine($"\n{statementCount,20} source statements.\n{syntaxErrorCount,20} syntax errors.\n{elapsedTime,20:F2} seconds total parsing time.\n");
                break;
        }
    }

    /*
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
    */
}
