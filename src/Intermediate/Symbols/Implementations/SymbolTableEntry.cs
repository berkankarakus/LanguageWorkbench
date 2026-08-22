using Intermediate.Symbols.Interfaces;

namespace Intermediate.Symbols.Implementations;

/// <summary>
/// Represents a single entry in a symbol table.
/// </summary>
public sealed class SymbolTableEntry(string name, ISymbolTable symbolTable) : ISymbolTableEntry
{
    private readonly List<int> _lineNumbers = [];
    private readonly Dictionary<ISymbolTableKey, object> _entries = new();

    /// <inheritdoc />
    public string Name { get; } = name;

    /// <inheritdoc />
    public ISymbolTable SymbolTable { get; } = symbolTable;

    /// <inheritdoc />
    public void AppendLineNumber(int lineNumber)
    {
        _lineNumbers.Add(lineNumber);
    }

    /// <inheritdoc />
    public List<int> LineNumbers => _lineNumbers;

    /// <inheritdoc />
    public void SetAttribute(ISymbolTableKey key, object value)
    {
        _entries.Add(key, value);
    }

    /// <inheritdoc />
    public object this[ISymbolTableKey key]
    {
        get => _entries[key];
        set => _entries[key] = value;
    }
}
