using System.Diagnostics;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Interfaces;
using Messaging;

namespace Backend.Interpreter;

/// <summary>
/// Executes intermediate code.
/// </summary>
public class Executor : BackendProcessor
{
    /// <inheritdoc />
    public override void Process(IIntermediateCode iCode, ISymbolTableStack symbolTableStack)
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
