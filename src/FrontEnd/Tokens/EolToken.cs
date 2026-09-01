using FrontEnd.Input;

namespace FrontEnd.Tokens;

/// <summary>
/// Represents the end of a source line.
/// </summary>
public class EolToken(SourceReader source) : Token(source)
{
    /// <summary>
    /// Initializes an end-of-line token with the specified token type.
    /// </summary>
    /// <param name="source">The source that reached the end of a line.</param>
    /// <param name="tokenType">The token type representing the end of the line.</param>
    public EolToken(SourceReader source, ITokenType tokenType) : this(source)
    {
        Type = tokenType;
    }

    /// <summary>
    /// Do nothing. Do not consume any source characters.
    /// </summary>
    protected override void Extract() { }
}
