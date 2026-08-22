namespace Intermediate.Symbols.Interfaces;

/// <summary>
/// Defines a named symbol declared in a symbol table.
/// </summary>
public interface ISymbolTableEntry
{
    /// <summary>Gets the source-level name of the symbol.</summary>
    /// <returns>The symbol name.</returns>
    string Name { get; }

    /// <summary>Gets the symbol table that owns the entry.</summary>
    /// <returns>The owning symbol table.</returns>
    ISymbolTable SymbolTable { get; }

    /// <summary>Records a source line that references the symbol.</summary>
    /// <param name="lineNumber">The one-based source line number.</param>
    void AppendLineNumber(int lineNumber);

    /// <summary>Gets the source lines that reference the symbol.</summary>
    /// <returns>The recorded source line numbers.</returns>
    List<int> LineNumbers { get; }

    /// <summary>Associates an attribute with the symbol.</summary>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">The attribute value.</param>
    void SetAttribute(ISymbolTableKey key, object value);

    /// <summary>Gets or sets an attribute associated with the symbol.</summary>
    /// <param name="key">The attribute key.</param>
    /// <returns>The stored attribute value.</returns>
    object this[ISymbolTableKey key] { get; set; }
}
