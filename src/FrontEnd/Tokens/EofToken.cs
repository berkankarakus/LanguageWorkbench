namespace FrontEnd.Tokens;

public class EofToken(Source.Source source) : Token(source)
{
    /// <summary>
    /// Do nothing. Do not consume any source characters.
    /// </summary>
    protected override void Extract() { }
}
