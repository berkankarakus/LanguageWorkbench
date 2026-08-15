using FrontEnd.Input;

namespace FrontEnd.Tokens;

public class EofToken(SourceReader source) : Token(source)
{
    /// <summary>
    /// Do nothing. Do not consume any source characters.
    /// </summary>
    protected override void Extract() { }
}
