using FrontEnd.Input;
using FrontEnd.Scanning;

namespace FrontEnd.Tokens;

/// <summary>
/// Represents a lexical token produced by a <see cref="Scanner"/>.
/// </summary>
public class Token(SourceReader source)
{
    /// <summary>
    /// Gets the type that classifies this token.
    /// </summary>
    public ITokenType? Type { get; protected set; }

    /// <summary>
    /// Gets the source text of this token.
    /// </summary>
    public string? Text { get; protected set; }

    /// <summary>
    /// Gets the value represented by this token, when applicable.
    /// </summary>
    public object? Value { get; protected set; }

    /// <summary>
    /// Gets the one-based line number at which this token begins.
    /// </summary>
    public int LineNumber { get; } = source.LineNumber;

    /// <summary>
    /// Gets the zero-based source position at which this token begins.
    /// </summary>
    public int Position { get; } = source.CurrentPosition; // position of the first token character

    /// <summary>
    /// Default method to extract only one-character tokens from the source.
    /// Subclasses can override this method to construct language-specific tokens. After extracting the token, the
    /// current source line position will be one beyond the last token character.
    /// </summary>
    protected virtual void Extract()
    {
        Text = char.ToString(CurrentChar());
    }

    /// <summary>
    /// Gets the current source character.
    /// </summary>
    /// <returns>The current source character.</returns>
    protected char CurrentChar()
    {
        return source.CurrentChar();
    }

    /// <summary>
    /// Gets the character following the current source character without consuming it.
    /// </summary>
    /// <returns>The next source character.</returns>
    protected char PeekChar()
    {
        return source.PeekChar();
    }

    /// <summary>
    /// Advances the source and returns the new current character.
    /// </summary>
    /// <returns>The new current source character.</returns>
    protected char NextChar()
    {
        return source.NextChar();
    }
}
