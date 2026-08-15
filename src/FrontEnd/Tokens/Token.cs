using FrontEnd.Input;
using FrontEnd.Scanning;

namespace FrontEnd.Tokens;

/// <summary>
/// Represents a lexical token produced by a <see cref="Scanner"/>.
/// </summary>
public class Token
{
    public ITokenType? TokenType { get; protected set; }
    public string? Text { get; protected set; }
    public object? Value { get; protected set; }
    private readonly SourceReader _source;
    public int LineNumber { get; }
    public int Position { get; } // position of the first token character

    public Token(SourceReader source)
    {
        _source = source;
        LineNumber = source.LineNumber;
        Position = source.CurrentPosition;
    }

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
        return _source.CurrentChar();
    }

    protected virtual char PeekChar()
    {
        return _source.PeekChar();
    }
}
