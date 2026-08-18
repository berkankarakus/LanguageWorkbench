using Messaging;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Writes source lines received from the front end to the console.
/// </summary>
public sealed class SourceMessageListener(Action flushTokens) : IMessageListener
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

                flushTokens();
                Console.WriteLine($"{lineNumber,3:D3} {lineText}");
                break;

            default:
                throw new ArgumentException("Invalid type argument.");
        }
    }
}
