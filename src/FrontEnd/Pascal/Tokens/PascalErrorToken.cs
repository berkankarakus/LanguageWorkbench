using FrontEnd.Input;
using FrontEnd.Pascal.ErrorHandling;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Represents a token produced for a Pascal lexical error.
/// </summary>
public class PascalErrorToken(SourceReader source) : PascalToken(source)
{
    /// <summary>
    /// Initializes a lexical-error token with its diagnostic code and source text.
    /// </summary>
    /// <param name="source">The source from which the token originated.</param>
    /// <param name="errorCode">The lexical error represented by the token.</param>
    /// <param name="tokenText">The source text that caused the error.</param>
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
