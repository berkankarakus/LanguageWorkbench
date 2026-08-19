namespace Intermediate.Symbols;

/// <summary>
/// Defines a lexical scope that holds symbols declared by a source program.
/// </summary>
public interface ISymbolTable
{
    /// <summary>
    /// Gets the lexical nesting level of this scope.
    /// </summary>
    /// <returns>The zero-based nesting level.</returns>
    int GetNestingLevel();

    /// <summary>
    /// Adds a symbol to this scope.
    /// </summary>
    /// <param name="name">The source-level name of the symbol.</param>
    /// <returns>The newly created entry.</returns>
    SymbolTableEntry Enter(string name);

    /// <summary>
    /// Finds a symbol declared in this scope.
    /// </summary>
    /// <param name="name">The source-level name of the symbol.</param>
    /// <returns>The matching entry.</returns>
    SymbolTableEntry? Lookup(string name);

    /// <summary>
    /// Gets the entries in ascending name order.
    /// </summary>
    /// <returns>The sorted symbol-table entries.</returns>
    List<SymbolTableEntry> SortedEntries();
}
