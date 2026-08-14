namespace FrontEnd;

/// <summary>
/// Represents a lexical token produced by a <see cref="Scanner"/>.
/// </summary>
public class Token
{
    protected ITokenType _tokenType; // language-specific token type
    protected string _text; // token text
    protected object _value; // token value
    protected Source.Source _source;
    protected int _lineNumber; // line number of the token's source file
    protected int _position; // position of the first token character

    public Token(Source.Source source)
    {
        _source = source;
        _lineNumber = source.LineNumber;
        _position = source.CurrentPosition;
    }

    /// <summary>
    /// Default method to extract only one-character tokens from the source.
    /// Subclasses can override this method to construct language-specific tokens. After extracting the token, the
    /// current source line position will be one beyond the last token character.
    /// </summary>
    protected virtual void Extract()
    {
        _text = char.ToString(CurrentChar());
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
