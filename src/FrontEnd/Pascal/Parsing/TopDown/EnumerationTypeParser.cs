using FrontEnd.Pascal.Types.Interfaces;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Parsing.TopDown;

public class EnumerationTypeParser(PascalParserTopDown parent) : PascalParserTopDown(parent)
{
    // Syncronization set to start an enumeration constant.

    public ITypeSpec? Parse(Token token)
    {
        throw new NotImplementedException();
    }
}
