using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;

namespace Intermediate.Symbols.Implementations;

/// <summary>
/// Represents the stack of symbol tables used to model nested lexical scopes.
/// </summary>
public class SymbolTableStack : ISymbolTableStack
{
    private int _currentNestingLevel;
    private readonly List<ISymbolTable> _symbolTable;

    /// <summary>
    /// Initializes a symbol-table stack with its outermost scope.
    /// </summary>
    public SymbolTableStack()
    {
        _currentNestingLevel = 0;
        _symbolTable =
        [
            SymbolTableFactory.CreateSymbolTable(_currentNestingLevel)
        ];
    }

    /// <inheritdoc />
    public int CurrentNestingLevel => _currentNestingLevel;

    /// <inheritdoc />
    public ISymbolTable LocalSymbolTable => _symbolTable[_currentNestingLevel];

    /// <inheritdoc />
    public ISymbolTableEntry EnterLocal(string name)
    {
        return _symbolTable[_currentNestingLevel].Enter(name);
    }

    /// <inheritdoc />
    public ISymbolTableEntry? LookupLocal(string name)
    {
        return _symbolTable[_currentNestingLevel].Lookup(name);
    }

    /// <inheritdoc />
    public ISymbolTableEntry? Lookup(string name)
    {
        return LookupLocal(name);
    }
}
