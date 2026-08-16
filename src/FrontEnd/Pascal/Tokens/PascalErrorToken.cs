using FrontEnd.Input;
using FrontEnd.Pascal.ErrorHandling;

namespace FrontEnd.Pascal.Tokens;

public class PascalErrorToken(SourceReader source) : PascalToken(source)
{
    public PascalErrorToken(SourceReader source, PascalErrorCode errorCode, string tokenText) : this(source)
    {
        Text = tokenText;
        Type = PascalTokenType.Error;
        Value = errorCode;
    }

    protected override void Extract()
    {
        // Do nothing.
    }
}
