using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Symbols.Implementations;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Pascal.Types.Interfaces;
using FrontEnd.Tokens;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal type definitions.
/// </summary>
public class TypeDefinitionsParser(PascalParserTopDown parent) : StatementParser(parent)
{
    // Synchronization set for a type identifier.
    private static readonly HashSet<ITokenType> IdentifierSet =
    [
        ..DeclarationsParser.VarStartSet,
        PascalTokenType.Identifier
    ];

    // Synchronization set for the = token.
    private static readonly HashSet<ITokenType> EqualsSet =
    [
        ..ConstantDefinitionsParser.ConstantStartSet,
        PascalTokenType.Equals,
        PascalTokenType.SemiColon
    ];

    // Synchronization set for what follows a definition or declaration.
    private static readonly HashSet<ITokenType> FollowSet = [PascalTokenType.SemiColon];

    // Syncronization set for the start of the next definition or declaration.
    private static readonly HashSet<ITokenType> NextStartSet =
    [
        ..DeclarationsParser.VarStartSet,
        PascalTokenType.SemiColon,
        PascalTokenType.Identifier
    ];

    public override IIntermediateCodeNode? Parse(Token token)
    {
        token = Synchronize(IdentifierSet);

        // Loop to parse a sequence of type definitions separated by semicolons.
        while (token.Type == PascalTokenType.Identifier)
        {
            string? name = token.Text?.ToLowerInvariant();

            if (name is null)
                break;

            ISymbolTableEntry? typeId = SymbolTableStack.LookupLocal(name);

            // Enter the new identifier into the symbol table but don't set how it's defined yet.
            if (typeId is null)
            {
                typeId = SymbolTableStack.EnterLocal(name);
                typeId.AppendLineNumber(token.LineNumber);
            }

            else
            {
                ErrorHandler.Flag(token, PascalErrorCode.IdentifierRedefined, this);
                typeId = null;
            }

            // Consume the identifier.
            _ = NextToken();

            // Syncronize on the = token.
            token = Synchronize(EqualsSet);

            if (token.Type == PascalTokenType.Equals)
                token = NextToken(); // consume the =

            else
                ErrorHandler.Flag(token, PascalErrorCode.MissingEquals, this);

            // Parse the type specification.
            TypeSpecificationParser typeSpecificationParser = new(this);
            ITypeSpec? typeSpec = typeSpecificationParser.Parse(token);

            // Set the identifier to be a type & set its type specification.
            typeId?[PascalSymbolTableKey.Definition] = Definition.Type;

            // Cross-link the type identifier & the type specification.
            if (typeSpec != null && typeId != null)
            {
                typeSpec.Identifier ??= typeId;
                typeId[PascalSymbolTableKey.TypeSpec] = typeSpec;
            }

            else
            {
                _ = Synchronize(FollowSet);
            }

            token = CurrentToken();
            ITokenType? tokenType = token.Type;

            // Look for one or more semicolons after a definition.
            if (tokenType == PascalTokenType.SemiColon)
            {
                while (token.Type == PascalTokenType.SemiColon)
                    token = NextToken();
            }

            // If at the start of the next definition or declaration, then missing a semicolon.
            else if (tokenType != null && NextStartSet.Contains(tokenType))
            {
                ErrorHandler.Flag(token, PascalErrorCode.MissingSemicolon, this);
            }

            token = Synchronize(IdentifierSet);
        }

        return null;
    }
}
