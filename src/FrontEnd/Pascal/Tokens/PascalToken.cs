using FrontEnd.Input;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Base class for Pascal token classes.
/// </summary>
public abstract class PascalToken(SourceReader source) : Token(source)
{
    /// <summary>
    /// Extracts this token after its derived instance has been constructed.
    /// </summary>
    internal void ExtractToken() => Extract();
}
