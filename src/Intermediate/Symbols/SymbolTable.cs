using Intermediate.Factory;

namespace Intermediate.Symbols;

/// <summary>
/// Represents the symbols declared within one lexical scope.
/// </summary>
public sealed class SymbolTable(int nestingLevel) : ISymbolTable
{
    private readonly SortedDictionary<string, SymbolTableEntry> _entries = new();

    /// <inheritdoc />
    public int GetNestingLevel()
    {
        return nestingLevel;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public SymbolTableEntry Enter(string name)
    {
        SymbolTableEntry entry = SymbolTableFactory.CreateSymbolTableEntry(name, symbolTable: this);
        _entries.Add(name, entry);

        return entry;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public SymbolTableEntry Lookup(string name)
    {
        return _entries[name];
    }

    /// <inheritdoc />
    public List<SymbolTableEntry> SortedEntries()
    {
        return [.. _entries.Values];
    }
}
