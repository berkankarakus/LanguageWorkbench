using Intermediate.Factory;

namespace Intermediate.Symbols;

/// <summary>
/// Represents the stack of symbol tables used to model nested lexical scopes.
/// </summary>
public class SymbolTableStack : ISymbolTableStack
{
    private int _currentNestingLevel;
    private readonly List<ISymbolTable> _symbolTable;

    public SymbolTableStack()
    {
        _currentNestingLevel = 0;
        _symbolTable =
        [
            SymbolTableFactory.CreateSymbolTable(_currentNestingLevel)
        ];
    }

    /// <inheritdoc />
    public int GetCurrentNestingLevel()
    {
        return _currentNestingLevel;
    }

    /// <inheritdoc />
    public ISymbolTable GetLocalSymbolTable()
    {
        return _symbolTable[_currentNestingLevel];
    }

    /// <inheritdoc />
    public SymbolTableEntry EnterLocal(string name)
    {
        return _symbolTable[_currentNestingLevel].Enter(name);
    }

    /// <inheritdoc />
    public SymbolTableEntry? LookupLocal(string name)
    {
        return _symbolTable[_currentNestingLevel].Lookup(name);
    }

    /// <inheritdoc />
    public SymbolTableEntry? Lookup(string name)
    {
        return LookupLocal(name);
    }
}
