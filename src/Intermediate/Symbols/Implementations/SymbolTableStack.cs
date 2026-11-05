using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;

namespace Intermediate.Symbols.Implementations;

/// <summary>
/// Represents the stack of symbol tables used to model nested lexical scopes.
/// </summary>
public sealed class SymbolTableStack : ISymbolTableStack
{
    private readonly Stack<ISymbolTable> _symbolTables;

    /// <summary>
    /// Initializes a symbol-table stack with its outermost scope.
    /// </summary>
    public SymbolTableStack()
    {
        _symbolTables = new();
        _symbolTables.Push(SymbolTableFactory.CreateSymbolTable(nestingLevel: 0));
    }

    /// <inheritdoc />
    public int CurrentNestingLevel => _symbolTables.Count - 1;

    /// <inheritdoc />
    public ISymbolTable LocalSymbolTable => _symbolTables.Peek();

    /// <inheritdoc />
    public ISymbolTableEntry? ProgramId { get; set; }

    /// <inheritdoc />
    public ISymbolTableEntry EnterLocal(string name)
    {
        return LocalSymbolTable.Enter(name);
    }

    /// <inheritdoc />
    public ISymbolTableEntry? LookupLocal(string name)
    {
        return LocalSymbolTable.Lookup(name);
    }

    /// <inheritdoc />
    public ISymbolTableEntry? Lookup(string name)
    {
        // Search the current and enclosing scopes.
        foreach (ISymbolTable symbolTable in _symbolTables)
        {
            ISymbolTableEntry? foundEntry = symbolTable.Lookup(name);

            if (foundEntry is not null)
                return foundEntry;
        }

        return null;
    }

    /// <inheritdoc />
    public ISymbolTable Push()
    {
        int nestingLevel = CurrentNestingLevel + 1;
        SymbolTable symbolTable = SymbolTableFactory.CreateSymbolTable(nestingLevel);
        _symbolTables.Push(symbolTable);

        return symbolTable;
    }

    public ISymbolTable Push(ISymbolTable symbolTable)
    {
        ArgumentNullException.ThrowIfNull(symbolTable);

        int nestingLevel = CurrentNestingLevel + 1;

        if (symbolTable.NestingLevel != nestingLevel)
            throw new ArgumentException("The pushed table must represent the next nesting level.", nameof(symbolTable));

        _symbolTables.Push(symbolTable);

        return symbolTable;
    }

    public ISymbolTable Pop()
    {
        if (_symbolTables.Count == 1)
            throw new InvalidOperationException("The outermost symbol table cannot be removed.");

        return _symbolTables.Pop();
    }
}
