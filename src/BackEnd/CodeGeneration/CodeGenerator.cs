using System.Diagnostics;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Interfaces;
using Messaging;

namespace Backend.CodeGeneration;

/// <summary>
/// Processes intermediate code and symbols to generate instructions.
/// </summary>
public class CodeGenerator : BackendProcessor
{
    /// <inheritdoc />
    public override void Process(IIntermediateCode iCode, ISymbolTableStack symbolTableStack)
    {
        long startTime = Stopwatch.GetTimestamp();
        float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;
        int instructionCount = 0;

        SendMessage(new Message(MessageType.CompilerSummary, new object?[] { instructionCount, elapsedTime }));
    }
}
