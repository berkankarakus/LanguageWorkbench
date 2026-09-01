using FrontEnd.Input;

namespace FrontEnd.Tokens;

/// <summary>
/// Represents the end of a source stream.
/// </summary>
public class EofToken(SourceReader source) : Token(source)
{
    /// <summary>
    /// Initializes an end-of-file token with the specified token type.
    /// </summary>
    /// <param name="source">The source that reached its end.</param>
    /// <param name="tokenType">The token type representing the end of the source.</param>
    public EofToken(SourceReader source, ITokenType tokenType) : this(source)
    {
        Type = tokenType;
    }

    /// <summary>
    /// Do nothing. Do not consume any source characters.
    /// </summary>
    protected override void Extract() { }
}
