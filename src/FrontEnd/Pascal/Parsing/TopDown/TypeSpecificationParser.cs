using FrontEnd.Pascal.Tokens;
using FrontEnd.Pascal.Types.Interfaces;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal type specifications.
/// </summary>
public class TypeSpecificationParser(PascalParserTopDown parent) : StatementParser(parent)
{
    // Syncronization set for starting a type specification.
    private static readonly HashSet<ITokenType> TypeStartSet =
    [
        ..SimpleTypeParser.SimpleTypeStartSet,
        PascalTokenType.Array,
        PascalTokenType.Record,
        PascalTokenType.SemiColon
    ];

    public static HashSet<ITokenType> TypeStartSet1 => TypeStartSet;

    /// <summary>
    /// Parses a type specification beginning at the current token.
    /// </summary>
    /// <param name="token">The first token of the type specification.</param>
    /// <returns>The parsed type specification, or <see langword="null"/> until type parsing is implemented.</returns>
    public new ITypeSpec? Parse(Token token)
    {
        // Syncronize at the start of a type specification.
        token = Synchronize(TypeStartSet1);

        if (token?.Type is null)
            return null;

        if (token.Type == PascalTokenType.Array)
        {
            ArrayTypeParser arrayTypeParser = new(this);
            return arrayTypeParser.Parse(token);
        }

        if (token.Type == PascalTokenType.Record)
        {
            RecordTypeParser recordTypeParser = new(this);
            return recordTypeParser.Parse(token);
        }

        SimpleTypeParser simpleTypeParser = new(this);
        return simpleTypeParser.Parse(token);
    }
}
