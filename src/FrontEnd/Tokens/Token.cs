using FrontEnd.Input;
using FrontEnd.Scanning;

namespace FrontEnd.Tokens;

/// <summary>
/// Represents a lexical token produced by a <see cref="Scanner"/>.
/// </summary>
public class Token(SourceReader source)
{
    public ITokenType? TokenType { get; protected set; }
    public string? Text { get; protected set; }
    public object? Value { get; protected set; }
    public int LineNumber { get; } = source.LineNumber;
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

    protected virtual char CurrentChar()
    {
        return source.CurrentChar();
    }

    protected virtual char PeekChar()
    {
        return source.PeekChar();
    }
}
