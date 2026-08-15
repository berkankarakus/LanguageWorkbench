using FrontEnd.Input;
using FrontEnd.Parsing;

namespace FrontEnd.Factory;

/// <summary>
/// Defines the factory contract for a language-specific front end.
/// </summary>
/// <typeparam name="TParser">The concrete parser type produced by the factory.</typeparam>
public abstract class FrontEndFactory<TParser> where TParser : Parser
{
    /// <summary>
    /// Creates a parser for the supplied source program.
    /// </summary>
    /// <param name="source">The source program to parse.</param>
    /// <returns>The configured parser type.</returns>
    public abstract TParser CreateParser(SourceReader source);
}
