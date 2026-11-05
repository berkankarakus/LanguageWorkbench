using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Interfaces;

namespace Backend.Interpreter;

/// <summary>
/// Holds the mutable state for one interpreter run.
/// </summary>
public sealed class ExecutionContext
{
    private readonly Dictionary<IIntermediateCodeNode, Dictionary<object, IIntermediateCodeNode>> _jumpTables = [];

    /// <summary>
    /// Gets the intermediate code being interpreted.
    /// </summary>
    public IIntermediateCode? Code { get; private set; }

    /// <summary>
    /// Gets the symbol-table stack associated with the intermediate code.
    /// </summary>
    public ISymbolTableStack? SymbolTableStack { get; private set; }

    /// <summary>
    /// Gets the runtime-error handler for the interpreter run.
    /// </summary>
    public RuntimeErrorHandler RuntimeErrors { get; } = new();

    /// <summary>
    /// Gets the number of statements executed during the interpreter run.
    /// </summary>
    public int ExecutionCount { get; private set; }

    /// <summary>
    /// Initializes this context for a new interpreter run.
    /// </summary>
    /// <param name="code">The intermediate code to execute.</param>
    /// <param name="symbolTableStack">The symbol-table stack produced by translation.</param>
    public void BeginRun(IIntermediateCode code, ISymbolTableStack symbolTableStack)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(symbolTableStack);

        Code = code;
        SymbolTableStack = symbolTableStack;
        ExecutionCount = 0;
        RuntimeErrors.Reset();
        _jumpTables.Clear();
    }

    /// <summary>
    /// Records execution of one statement.
    /// </summary>
    public void IncrementExecutionCount()
    {
        ++ExecutionCount;
    }

    /// <summary>
    /// Gets a cached CASE jump table, building it once for the supplied SELECT node.
    /// </summary>
    /// <param name="node">The SELECT node that owns the table.</param>
    /// <param name="createTable">Creates the table when it is not cached.</param>
    /// <returns>The jump table for <paramref name="node"/>.</returns>
    internal Dictionary<object, IIntermediateCodeNode> GetOrCreateJumpTable(
        IIntermediateCodeNode node,
        Func<IIntermediateCodeNode, Dictionary<object, IIntermediateCodeNode>> createTable
    )
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(createTable);

        if (_jumpTables.TryGetValue(node, out Dictionary<object, IIntermediateCodeNode>? jumpTable))
            return jumpTable;

        jumpTable = createTable(node);
        _jumpTables.Add(node, jumpTable);
        return jumpTable;
    }
}
