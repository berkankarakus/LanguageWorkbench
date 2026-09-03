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
    private readonly Executor _rootExecutor;
    private int _executionCount;

    /// <summary>
    /// Initializes the root interpreter executor.
    /// </summary>
    public Executor()
    {
        _rootExecutor = this;
    }

    /// <summary>
    /// Initializes a child executor with the state of its parent executor.
    /// </summary>
    /// <param name="parent">The executor whose intermediate code and symbol tables are reused.</param>
    protected Executor(Executor parent)
    {
        ArgumentNullException.ThrowIfNull(parent);
        _rootExecutor = parent._rootExecutor;
        Code = parent.Code;
        SymbolTableStack = parent.SymbolTableStack;
    }

    /// <inheritdoc />
    public override void Process(
        IIntermediateCode intermediateCode,
        ISymbolTableStack symbolTableStack
    )
    {
        Code = intermediateCode;
        SymbolTableStack = symbolTableStack;
        _executionCount = 0;

        long startTime = Stopwatch.GetTimestamp();

        // Get the root node of the intermediate code & execute.
        IIntermediateCodeNode? rootNode = intermediateCode.Root;
        StatementExecutor statementExecutor = new(parent: this);
        statementExecutor.Execute(rootNode);

        float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;
        int executionCount = _executionCount;
        int runtimeErrors = RuntimeErrorHandler.ErrorCount;

        SendMessage(
            new Message(
                MessageType.InterpreterSummary,
                body: new object?[] { executionCount, runtimeErrors, elapsedTime }
            )
        );
    }

    /// <summary>
    /// Records execution of one statement in the current interpreter run.
    /// </summary>
    protected void IncrementExecutionCount()
    {
        ++_rootExecutor._executionCount;
    }
}
