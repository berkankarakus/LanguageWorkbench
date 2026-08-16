using FrontEnd.Input;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Base class for Pascal token classes.
/// </summary>
/// <param name="source"></param>
public abstract class PascalToken(SourceReader source) : Token(source);