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
    public override void Process(ICode iCode, ISymbolTable symbolTable)
    {
        long startTime = DateTime.UtcNow.Ticks;
        float elapsedTime = (DateTime.UtcNow.Ticks - startTime) / 1000f;
        int instructionCount = 0;

        SendMessage(new Message(MessageType.CompilerSummary, new object[] { instructionCount, elapsedTime }));
    }
}
