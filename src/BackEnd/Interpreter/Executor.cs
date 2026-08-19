using System.Diagnostics;
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
    public override void Process(ICode iCode, ISymbolTableStack symbolTableStack)
    {
        long startTime = Stopwatch.GetTimestamp();
        float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;
        int executionCount = 0;
        int runtimeErrors = 0;

        SendMessage(new Message(
            MessageType.InterpreterSummary,
            new object?[] { executionCount, runtimeErrors, elapsedTime }));
    }
}
