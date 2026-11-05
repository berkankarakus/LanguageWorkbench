using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Symbols.Implementations;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Pascal.Types.Interfaces;
using FrontEnd.Tokens;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Simple type parser.
/// </summary>
public class SimpleTypeParser(PascalParserTopDown parent) : PascalParserTopDown(parent)
{
    // Syncronization set for starting a simple type specification.
    public static readonly HashSet<ITokenType> SimpleTypeStartSet =
    [
        ..ConstantDefinitionsParser.ConstantStartSet,
        PascalTokenType.LeftParen,
        PascalTokenType.Comma,
        PascalTokenType.SemiColon
    ];

    /// <summary>
    /// Parse a simple Pascal type specification.
    /// </summary>
    /// <param name="token">The current token</param>
    /// <returns>Type simple type specification</returns>
    public ITypeSpec? Parse(Token token)
    {
        // Syncronize at the start of a simple type specification.
        token = Synchronize(SimpleTypeStartSet);

        if (token?.Type is null)
            return null;

        if (token.Type == PascalTokenType.Identifier)
        {
            string? name = token.Text?.ToLowerInvariant();
            ISymbolTableEntry? id = name is null
                ? null
                : SymbolTableStack.Lookup(name);

            if (id == null)
            {
                ErrorHandler.Flag(token, PascalErrorCode.IdentifierUndefined, this);
                _ = NextToken(); // consume the identifier

                return null;
            }

            else
            {
                Definition? definition = (Definition?)id[PascalSymbolTableKey.Definition];

                // It's either a type identifier or the start of a subrange type.
                if (definition == Definition.Type)
                {
                    id.AppendLineNumber(token.LineNumber);
                    _ = NextToken(); // consume the identifier

                    // Return the type of the referent type.
                    return (ITypeSpec?)id[PascalSymbolTableKey.TypeSpec];
                }

                if (definition != Definition.Constant && definition != Definition.EnumerationConstant)
                {
                    ErrorHandler.Flag(token, PascalErrorCode.NotTypeIdentifier, this);
                    _ = NextToken(); // consume the identifier

                    return null;
                }

                return ParseSubrangeType(token);
            }
        }

        if (token.Type == PascalTokenType.LeftParen)
        {
            EnumerationTypeParser enumerationTypeParser = new(this);
            return enumerationTypeParser.Parse(token);
        }

        if (token.Type == PascalTokenType.Comma || token.Type == PascalTokenType.SemiColon)
        {
            ErrorHandler.Flag(token, PascalErrorCode.InvalidType, this);
            return null;
        }

        return ParseSubrangeType(token);
    }

    private ITypeSpec? ParseSubrangeType(Token token)
    {
        SubrangeTypeParser subrangeTypeParser = new(this, PascalPredefined);
        return subrangeTypeParser.Parse(token);
    }
}
