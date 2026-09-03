using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Messaging;

namespace Backend.Interpreter;

/// <summary>
/// Reports runtime errors raised while interpreting intermediate code.
/// </summary>
public static class RuntimeErrorHandler
{
    private const int MaxErrors = 5;
    public static int ErrorCount { get; private set; }

    /// <summary>
    /// Reports a runtime error and associates it with the nearest enclosing source line.
    /// </summary>
    /// <param name="codeNode">The node at which execution failed.</param>
    /// <param name="errorCode">The reported runtime error.</param>
    /// <param name="backend">The backend that publishes the error message.</param>
    public static void Flag(IIntermediateCodeNode? codeNode, RuntimeErrorCode errorCode, BackendProcessor backend)
    {
        // Look for the ancestor statement node with a line number attribute.
        while (codeNode is not null && codeNode[IntermediateCodeKey.Line] is null)
            codeNode = codeNode.Parent;

        if (codeNode?[IntermediateCodeKey.Line] is int lineNumber)
        {
            backend.SendMessage(new Message(
                MessageType.RuntimeError,
                body: new object[]
                {
                    errorCode.ToString(),
                    lineNumber
                }
            ));
        }

        if (++ErrorCount <= MaxErrors)
            return;

        Console.WriteLine("*** ABORTED AFTER TOO MANY RUNTIME ERRORS ***");
        Environment.Exit(-1);
    }
}
