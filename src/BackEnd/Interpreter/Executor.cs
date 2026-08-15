using Intermediate.Code;
using Intermediate.Symbols;
using Messaging;

namespace Backend.Interpreter;

/// <summary>
/// Executes intermediate code.
/// </summary>
public class Executor : BackendProcessor
{
    /// <inheritdoc />
    public override void Process(ICode iCode, ISymbolTable symbolTable)
    {
        long startTime = DateTime.UtcNow.Ticks;
        float elapsedTime = (DateTime.UtcNow.Ticks - startTime) / 1000f;
        int executionCount = 0;
        int runtimeErrors = 0;

        SendMessage(new Message(
            MessageType.InterpreterSummary,
            new object[] { executionCount, runtimeErrors, elapsedTime }));
    }
}
