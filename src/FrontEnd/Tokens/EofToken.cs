using FrontEnd.Input;

namespace FrontEnd.Tokens;

public class EofToken(SourceReader source) : Token(source)
{
    public EofToken(SourceReader source, ITokenType tokenType) : this(source)
    {
        Type = tokenType;
    }

    /// <summary>
    /// Do nothing. Do not consume any source characters.
    /// </summary>
    protected override void Extract() { }
}
