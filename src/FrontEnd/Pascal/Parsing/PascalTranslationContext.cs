using FrontEnd.Parsing;
using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Types.Implementations;

namespace FrontEnd.Pascal.Parsing;

/// <summary>
/// Holds the Pascal-specific state used by parsers in one translation.
/// </summary>
public sealed class PascalTranslationContext : TranslationContext
{
    /// <summary>
    /// Gets the predefined Pascal types and constants for this translation.
    /// </summary>
    public Predefined Predefined { get; } = new();

    /// <summary>
    /// Gets the syntax-error handler for the translation.
    /// </summary>
    public PascalErrorHandler ErrorHandler { get; } = new();
}
