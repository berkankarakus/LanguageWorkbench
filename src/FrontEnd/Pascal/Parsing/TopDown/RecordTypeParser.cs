using FrontEnd.Pascal.Types.Interfaces;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Parsing.TopDown;

public class RecordTypeParser(PascalParserTopDown parent) : PascalParserTopDown(parent)
{
    public ITypeSpec? Parse(Token token)
    {
        throw new NotImplementedException();
    }
}
