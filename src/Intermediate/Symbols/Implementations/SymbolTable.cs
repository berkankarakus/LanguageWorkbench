using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;

namespace Intermediate.Symbols.Implementations;

/// <summary>
/// Represents the symbols declared within one lexical scope.
/// </summary>
public sealed class SymbolTable(int nestingLevel) : ISymbolTable
{
    private readonly SortedDictionary<string, ISymbolTableEntry> _entries = new();

    /// <inheritdoc />
    public int NestingLevel { get; } = nestingLevel;

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public ISymbolTableEntry Enter(string name)
    {
        ISymbolTableEntry entry = SymbolTableFactory.CreateSymbolTableEntry(name, symbolTable: this);
        _entries.Add(name, entry);

        return entry;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public ISymbolTableEntry? Lookup(string name)
    {
        return _entries.GetValueOrDefault(name);
    }

    /// <inheritdoc />
    public List<ISymbolTableEntry> SortedEntries()
    {
        return [.. _entries.Values];
    }
}
