using FrontEnd.Pascal.Types.Interfaces;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Parsing.TopDown;

public class ArrayTypeParser(PascalParserTopDown parent) : PascalParserTopDown(parent)
{
    internal ITypeSpec? Parse(Token token)
    {
        throw new NotImplementedException();
    }
}
