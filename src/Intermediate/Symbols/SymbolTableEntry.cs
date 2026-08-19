namespace Intermediate.Symbols;

/// <summary>
/// Represents a single entry in a symbol table.
/// </summary>
public sealed class SymbolTableEntry(string name, ISymbolTable symbolTable) : ISymbolTableEntry
{
    private readonly List<int> _lineNumbers = [];
    private readonly Dictionary<ISymbolTableKey, object> _entries = new();

    /// <inheritdoc />
    public string GetName()
    {
        return name;
    }

    /// <inheritdoc />
    public ISymbolTable GetSymbolTable()
    {
        return symbolTable;
    }

    /// <inheritdoc />
    public void AppendLineNumber(int lineNumber)
    {
        _lineNumbers.Add(lineNumber);
    }

    /// <inheritdoc />
    public List<int> GetLineNumbers()
    {
        return _lineNumbers;
    }

    /// <inheritdoc />
    public void SetAttribute(ISymbolTableKey key, object value)
    {
        _entries.Add(key, value);
    }

    /// <inheritdoc />
    public object GetAttribute(ISymbolTableKey key)
    {
        return _entries[key];
    }
}
