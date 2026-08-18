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
    /// <param name="message">The back-end summary message to render.</param>
    public void MessageReceived(Message message)
    {
        switch (message.Type)
        {
            case MessageType.InterpreterSummary when message.Body is object?[] and [int executionCount, int runtimeErrorCount, float elapsedTime]:
                Console.WriteLine($"\n{executionCount,10} statements executed.\n{runtimeErrorCount,10} runtime errors.\n{elapsedTime,10:F2} seconds total execution time.");
                break;

            case MessageType.CompilerSummary when message.Body is object?[] and [int instructionCount, float elapsedTime]:
                Console.WriteLine($"\n{instructionCount,10} instructions generated.\n{elapsedTime,10:F2} seconds total code generation time.");
                break;
        }
    }
}
