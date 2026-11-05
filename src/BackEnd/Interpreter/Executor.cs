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
    private readonly ExecutionContext _executionContext;

    /// <summary>
    /// Initializes the root interpreter executor.
    /// </summary>
    public Executor()
    {
        _executionContext = new();
    }

    /// <summary>
    /// Initializes a child executor with the state of its parent executor.
    /// </summary>
    /// <param name="parent">The executor whose intermediate code and symbol tables are reused.</param>
    protected Executor(Executor parent)
        : base(parent)
    {
        ArgumentNullException.ThrowIfNull(parent);
        _executionContext = parent._executionContext;
    }

    /// <inheritdoc />
    public override void Process(
        IIntermediateCode intermediateCode,
        ISymbolTableStack symbolTableStack
    )
    {
        _executionContext.BeginRun(intermediateCode, symbolTableStack);

        long startTime = Stopwatch.GetTimestamp();

        // Get the root node of the intermediate code & execute.
        IIntermediateCodeNode? rootNode = intermediateCode.Root;
        StatementExecutor statementExecutor = new(parent: this);
        statementExecutor.Execute(rootNode);

        float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;
        int executionCount = _executionContext.ExecutionCount;
        int runtimeErrors = _executionContext.RuntimeErrors.ErrorCount;

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
        _executionContext.IncrementExecutionCount();
    }

    /// <summary>
    /// Gets the runtime error handler shared by executors in this interpreter run.
    /// </summary>
    protected RuntimeErrorHandler RuntimeErrors => _executionContext.RuntimeErrors;

    /// <summary>
    /// Gets the execution state shared by executors in the current interpreter run.
    /// </summary>
    protected ExecutionContext ExecutionContext => _executionContext;
}
