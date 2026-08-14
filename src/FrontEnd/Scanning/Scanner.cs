using FrontEnd.Tokens;

namespace FrontEnd.Scanning;

/// <summary>
/// Provides a language-independent base for lexical scanners.
/// </summary>
/// <remarks>
/// Language-specific scanners derive from this class and implement token extraction.
/// </remarks>
public abstract class Scanner(Source.Source source)
{
    private Token? _currentToken;

    /// <summary>
    /// Gets the most recently extracted token.
    /// </summary>
    /// <returns>The current token, or <see langword="null"/> before the first token is extracted.</returns>
    public virtual Token? CurrentToken()
    {
        return _currentToken;
    }

    /// <summary>
    /// Extracts and returns the next token from the source.
    /// </summary>
    /// <returns>The next extracted token.</returns>
    public virtual Token NextToken()
    {
        _currentToken = ExtractToken();
        return _currentToken;
    }

    /// <summary>
    /// Extracts the next token from the source.
    /// </summary>
    /// <returns>The extracted token.</returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when a derived scanner does not provide an implementation.
    /// </exception>
    protected abstract Token ExtractToken();

    /// <summary>
    /// Returns the current character from the source.
    /// </summary>
    /// <returns>The current character.</returns>
    public char CurrentChar()
    {
        return source.CurrentChar();
    }

    /// <summary>
    /// Advances the source and returns its next character.
    /// </summary>
    /// <returns>The next character.</returns>
    public char NextChar()
    {
        return source.NextChar();
    }
}
