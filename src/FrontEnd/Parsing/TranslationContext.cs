using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;
using Messaging;

namespace FrontEnd.Parsing;

/// <summary>
/// Holds the language-independent state produced during one source translation.
/// </summary>
public class TranslationContext
{
    /// <summary>
    /// Initializes state for a new translation.
    /// </summary>
    public TranslationContext()
    {
        MessageHandler = new();
        Code = new IntermediateCode();
        SymbolTableStack = SymbolTableFactory.CreateSymbolTableStack();
        SymbolTable = SymbolTableStack.LocalSymbolTable;
    }

    /// <summary>
    /// Gets the message publisher shared by parsers in this translation.
    /// </summary>
    internal MessageHandler MessageHandler { get; }

    /// <summary>
    /// Gets the intermediate code generated during translation.
    /// </summary>
    public IIntermediateCode Code { get; }

    /// <summary>
    /// Gets the root symbol table for the translation.
    /// </summary>
    public ISymbolTable SymbolTable { get; }

    /// <summary>
    /// Gets the symbol-table stack for the translation.
    /// </summary>
    public ISymbolTableStack SymbolTableStack { get; }
}
