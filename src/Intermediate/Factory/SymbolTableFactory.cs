using Intermediate.Symbols.Implementations;
using Intermediate.Symbols.Interfaces;

namespace Intermediate.Factory;

/// <summary>
/// Creates the symbol-table objects used by the intermediate representation.
/// </summary>
public static class SymbolTableFactory
{
    /// <summary>
    /// Creates a symbol-table stack with its root scope.
    /// </summary>
    /// <returns>A new symbol-table stack.</returns>
    public static SymbolTableStack CreateSymbolTableStack()
    {
        return new SymbolTableStack();
    }

    /// <summary>
    /// Creates a symbol table for the specified nesting level.
    /// </summary>
    /// <param name="nestingLevel">The lexical nesting level represented by the table.</param>
    /// <returns>A new symbol table.</returns>
    public static SymbolTable CreateSymbolTable(int nestingLevel)
    {
        return new SymbolTable(nestingLevel);
    }

    /// <summary>
    /// Creates an entry owned by a symbol table.
    /// </summary>
    /// <param name="name">The source-level name of the symbol.</param>
    /// <param name="symbolTable">The table that owns the entry.</param>
    /// <returns>A new symbol-table entry.</returns>
    public static SymbolTableEntry CreateSymbolTableEntry(string name, ISymbolTable symbolTable)
    {
        return new SymbolTableEntry(name, symbolTable);
    }
}
