using System.Diagnostics;
using Intermediate.Code;
using Intermediate.Symbols;
using Messaging;

namespace Backend.CodeGeneration;

/// <summary>
/// Processes intermediate code and symbols to generate instructions.
/// </summary>
public class CodeGenerator : BackendProcessor
{
    /// <inheritdoc />
    public override void Process(ICode iCode, ISymbolTableStack symbolTableStack)
    {
        long startTime = Stopwatch.GetTimestamp();
        float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;
        int instructionCount = 0;

        SendMessage(new Message(MessageType.CompilerSummary, new object?[] { instructionCount, elapsedTime }));
    }
}
