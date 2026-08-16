using Messaging;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Listener for back-end messages.
/// </summary>
public class BackendMessageListener : IMessageListener
{
    /// <summary>
    /// Called by the back-end whenever it produces a message.
    /// </summary>
    /// <param name="message"></param>
    public void MessageReceived(Message message)
    {
        object[] body;
        float elapsedTime;
        MessageType type = message.Type;

        switch (type)
        {
            case MessageType.InterpreterSummary:
                body = (object[])message.Body;
                int executionCount = (int)body[0];
                int runtimeErrorCount = (int)body[1];
                elapsedTime = (float)body[2];

                Console.WriteLine($"\n{executionCount,20} statements executed.\n{runtimeErrorCount,20} runtime errors.\n{elapsedTime,20:F2} seconds total execution time.\n");
                break;

            case MessageType.CompilerSummary:
                body = (object[])message.Body;
                int instructionCount = (int)body[0];
                elapsedTime = (float)body[1];

                Console.WriteLine($"\n{instructionCount,20} instructions generated.\n{elapsedTime,20:F2} seconds total code generation time.\n");
                break;
        }
    }
}
