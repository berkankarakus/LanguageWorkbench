using FrontEnd.Input;
using FrontEnd.Tokens;

namespace FrontEnd.Scanning;

/// <summary>
/// Provides a language-independent base for lexical scanners.
/// </summary>
/// <remarks>
/// Language-specific scanners derive from this class and implement token extraction.
/// </remarks>
public abstract class Scanner(SourceReader source)
{
    private Token? _currentToken;

    /// <summary>
    /// Gets the source reader scanned by this scanner.
    /// </summary>
    protected SourceReader Source { get; } = source;

    /// <summary>
    /// Gets the most recently extracted token.
    /// </summary>
    /// <returns>The current token, or <see langword="null"/> before the first token is extracted.</returns>
    public Token? CurrentToken()
    {
        return _currentToken;
    }

    /// <summary>
    /// Extracts and returns the next token from the source.
    /// </summary>
    /// <returns>The next extracted token.</returns>
    public Token NextToken()
    {
        _currentToken = ExtractToken();
        return _currentToken;
    }

    /// <summary>
    /// Extracts the next token from the source.
    /// </summary>
    /// <returns>The extracted token.</returns>
    protected abstract Token ExtractToken();

    /// <summary>
    /// Returns the current character from the source.
    /// </summary>
    /// <returns>The current character.</returns>
    protected char CurrentChar()
    {
        return Source.CurrentChar();
    }

    /// <summary>
    /// Advances the source and returns its next character.
    /// </summary>
    /// <returns>The next character.</returns>
    protected char NextChar()
    {
        return Source.NextChar();
    }
}
