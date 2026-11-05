using FrontEnd.Pascal.Symbols.Implementations;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Interfaces;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal's CONST, VAR and TYPE declarations.
/// </summary>
public class DeclarationsParser(PascalParserTopDown parent) : StatementParser(parent)
{
    private static readonly HashSet<ITokenType> DeclarationStartSet =
    [
        PascalTokenType.Const,
        PascalTokenType.Type,
        PascalTokenType.Var,
        PascalTokenType.Procedure,
        PascalTokenType.Function,
        PascalTokenType.Begin
    ];

    public static readonly HashSet<ITokenType> TypeStartSet =
    [
        .. DeclarationStartSet
    ];

    internal static readonly HashSet<ITokenType> VarStartSet =
    [
        .. TypeStartSet
    ];

    private static readonly HashSet<ITokenType> RoutineStartSet =
    [
        .. VarStartSet
    ];

    static DeclarationsParser()
    {
        TypeStartSet.Remove(PascalTokenType.Const);
        VarStartSet.Remove(PascalTokenType.Type);
        RoutineStartSet.Remove(PascalTokenType.Var);
    }

    public override IIntermediateCodeNode? Parse(Token token)
    {
        token = Synchronize(DeclarationStartSet);

        if (token.Type == PascalTokenType.Const)
        {
            token = NextToken(); // Consume CONST.

            ConstantDefinitionsParser constantDefinitionsParser = new(this);
            constantDefinitionsParser.Parse(token);
        }

        token = Synchronize(TypeStartSet);

        if (token.Type == PascalTokenType.Type)
        {
            token = NextToken(); // consume TYPE.

            TypeDefinitionsParser typeDefinitionsParser = new(this);
            typeDefinitionsParser.Parse(token);
        }

        token = Synchronize(VarStartSet);

        if (token.Type == PascalTokenType.Var)
        {
            token = NextToken(); // Consume VAR.

            VariableDeclarationsParser variableDeclarationsParser = new(this);
            variableDeclarationsParser.SetDefinition(Definition.Variable);
            variableDeclarationsParser.Parse(token);
        }

        _ = Synchronize(RoutineStartSet);

        return null;
    }
}
