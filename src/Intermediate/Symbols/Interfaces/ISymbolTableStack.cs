namespace Intermediate.Symbols.Interfaces;

/// <summary>
/// Defines a stack of symbol tables that represents the nesting scopes of a source program.
/// </summary>
public interface ISymbolTableStack
{
    /// <summary>
    /// Gets the nesting level of the current local scope.
    /// </summary>
    /// <returns>The zero-based nesting level of the local symbol table.</returns>
    int CurrentNestingLevel { get; }

    /// <summary>
    /// Gets the symbol table for the current local scope.
    /// </summary>
    /// <returns>The symbol table at the current nesting level.</returns>
    ISymbolTable LocalSymbolTable { get; }

    /// <summary>
    /// Adds a symbol with the specified name to the current local scope.
    /// </summary>
    /// <param name="name">The source-level name of the symbol.</param>
    /// <returns>The newly created entry for the symbol.</returns>
    ISymbolTableEntry EnterLocal(string name);

    /// <summary>
    /// Finds a symbol by name in the current local scope.
    /// </summary>
    /// <param name="name">The source-level name of the symbol.</param>
    /// <returns>The matching symbol entry.</returns>
    ISymbolTableEntry? LookupLocal(string name);

    /// <summary>
    /// Finds a symbol by name in the current scope or one of its enclosing scopes.
    /// </summary>
    /// <param name="name">The source-level name of the symbol.</param>
    /// <returns>The matching symbol entry, or <see langword="null"/> when no visible symbol has that name.</returns>
    ISymbolTableEntry? Lookup(string name);

    /// <summary>
    /// Gets or sets the symbol-table entry for the main program identifier.
    /// </summary>
    /// <remarks>
    /// The value is <see langword="null"/> until translation identifies the main program.
    /// </remarks>
    ISymbolTableEntry? ProgramId { get; set; }

    /// <summary>
    /// Pushes a new symbol table for the next nested lexical scope.
    /// </summary>
    /// <returns>The pushed symbol table.</returns>
    ISymbolTable Push();

    /// <summary>
    /// Pushes a symbol table for the next nested lexical scope.
    /// </summary>
    /// <param name="symbolTable">The symbol table to push.</param>
    /// <returns>The pushed symbol table.</returns>
    ISymbolTable Push(ISymbolTable symbolTable);

    /// <summary>
    /// Removes and returns the current nested symbol table.
    /// </summary>
    /// <returns>The removed symbol table.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to remove the outermost symbol table.
    /// </exception>
    ISymbolTable Pop();
}
