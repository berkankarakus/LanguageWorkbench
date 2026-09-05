using Messaging;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Renders messages produced by a compiler or interpreter back end.
/// </summary>
public class BackendMessageListener : IMessageListener
{
    private bool _isFirstOutputMessage = true;

    /// <summary>
    /// Renders a back-end message when its payload has the expected shape.
    /// </summary>
    /// <param name="message">The back-end message to render.</param>
    public void MessageReceived(Message message)
    {
        switch (message.Type)
        {
            case MessageType.InterpreterSummary
                when message.Body
                    is object?[]
                        and [int executionCount, int runtimeErrorCount, float elapsedTime]:
                Console.WriteLine(
                    $"\n{executionCount,10} statements executed.\n{runtimeErrorCount,10} runtime errors.\n{elapsedTime,10:F2} seconds total execution time."
                );
                break;

            case MessageType.CompilerSummary
                when message.Body is object?[] and [int instructionCount, float elapsedTime]:
                Console.WriteLine(
                    $"\n{instructionCount,10} instructions generated.\n{elapsedTime,10:F2} seconds total code generation time."
                );
                break;

            case MessageType.Assign
                when message.Body is object?[] and [int lineNumber, string variableName, var value]:
                PrintAssignment(lineNumber, variableName, value);
                break;

            case MessageType.RuntimeError
                when message.Body is object?[] and [string errorMessage, int lineNumber]:
                Console.WriteLine($"*** RUNTIME ERROR AT LINE {lineNumber:000}: {errorMessage}");
                break;
        }
    }

    private void PrintAssignment(int lineNumber, string variableName, object? value)
    {
        if (_isFirstOutputMessage)
        {
            Console.WriteLine("\n===== OUTPUT =====");
            _isFirstOutputMessage = false;
        }

        Console.WriteLine($" >>> LINE {lineNumber:000}: {variableName} = {value}");
    }
}
