using FrontEnd.Input;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Tokens;

public abstract class PascalToken(SourceReader source) : Token(source);