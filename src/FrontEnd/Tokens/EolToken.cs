using FrontEnd.Input;

namespace FrontEnd.Tokens;

/// <summary>
/// Represents the end of a source line.
/// </summary>
public class EolToken(SourceReader source) : Token(source)
{
    public EolToken(SourceReader source, ITokenType tokenType) : this(source)
    {
        Type = tokenType;
    }

    /// <summary>
    /// Do nothing. Do not consume any source characters.
    /// </summary>
    protected override void Extract() { }
}
