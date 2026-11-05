using System.Numerics;
using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Symbols.Implementations;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Pascal.Types.Factory;
using FrontEnd.Pascal.Types.Implementations;
using FrontEnd.Pascal.Types.Interfaces;
using FrontEnd.Tokens;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Implementations;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal constant definitions.
/// </summary>
public class ConstantDefinitionsParser(PascalParserTopDown parent) : StatementParser(parent)
{
    // Syncronization set for a constant identifier.
    private static readonly HashSet<ITokenType> IdentifierSet =
    [
        .. StatementFollowSet,
        PascalTokenType.Identifier
    ];

    // Syncronization set for starting a constant.
    internal static readonly HashSet<ITokenType> ConstantStartSet =
    [
        PascalTokenType.Identifier,
        PascalTokenType.Integer,
        PascalTokenType.Real,
        PascalTokenType.Plus,
        PascalTokenType.Minus,
        PascalTokenType.String,
        PascalTokenType.SemiColon
    ];

    // Syncronization set for the = token.
    private static readonly HashSet<ITokenType> EqualsSet =
    [
        .. ConstantStartSet,
        PascalTokenType.Equals,
        PascalTokenType.SemiColon
    ];

    // Synchronization set for the start of the next definition or declaration.
    private static readonly HashSet<ITokenType> NextStartSet =
    [
        ..DeclarationsParser.TypeStartSet,
        PascalTokenType.SemiColon,
        PascalTokenType.Identifier
    ];

    public override IIntermediateCodeNode? Parse(Token token)
    {
        token = Synchronize(IdentifierSet);

        // Loop to parse a sequence of constant definitions separated by semicolons.
        while (token.Type == PascalTokenType.Identifier)
        {
            string? name = token.Text?.ToLowerInvariant();
            ISymbolTableEntry? constantId = name is null
                ? null
                : SymbolTableStack.LookupLocal(name);

            // Enter the new identifier into the symbol table but don't set how it's defined yet.
            if (name is not null && constantId is null)
            {
                constantId = SymbolTableStack.EnterLocal(name);
                constantId.AppendLineNumber(token.LineNumber);
            }

            else
            {
                ErrorHandler.Flag(token, PascalErrorCode.IdentifierRedefined, this);
                constantId = null;
            }

            // Consume the identifier token.
            _ = NextToken();

            // Synchronize on the = token.
            token = Synchronize(EqualsSet);

            if (token.Type == PascalTokenType.Equals)
                token = NextToken(); // consume the =
            else
                ErrorHandler.Flag(token, PascalErrorCode.MissingEquals, this);

            // Parse the constant value.
            Token constantToken = token;
            object? value = ParseConstant(token);

            // Set identifier to be a constant & set its value.
            if (constantId != null)
            {
                constantId[PascalSymbolTableKey.Definition] = Definition.Constant;
                constantId[SymbolTableKey.ConstantValue] = value;

                // Set the constant's type.
                ITypeSpec constantType = constantToken.Type == PascalTokenType.Identifier
                    ? GetConstantType(constantToken)
                    : GetConstantType(value);

                constantId[PascalSymbolTableKey.TypeSpec] = constantType;
            }

            // ParseConstant advances the scanner, so refresh the local token before
            // deciding whether another definition follows this one.
            token = CurrentToken();

            // A definition normally ends with one or more semicolons.  Consume them
            // here so the loop can continue at the next identifier or declaration.
            if (token.Type == PascalTokenType.SemiColon)
            {
                do
                {
                    token = NextToken();
                }
                while (token.Type == PascalTokenType.SemiColon);
            }

            // An identifier at this point starts another definition and means that
            // the preceding semicolon was omitted.
            else if (token.Type != null && NextStartSet.Contains(token.Type))
            {
                ErrorHandler.Flag(token, PascalErrorCode.MissingSemicolon, this);
            }
        }

        token = CurrentToken();

        // Synchronize only when the token is not already a valid declaration
        // boundary.  In particular, BEGIN must be left for the statement parser.
        if (token.Type is null || !NextStartSet.Contains(token.Type))
            _ = Synchronize(IdentifierSet);

        return null;
    }

    /// <summary>
    /// Gets the type of a constant from its value.
    /// </summary>
    /// <param name="value">The constant value</param>
    /// <returns>
    /// The matching predefined type, a Pascal string type, or the predefined undefined type when the value is
    /// not a supported constant value.
    /// </returns>
    internal ITypeSpec GetConstantType(object? value)
    {
        Predefined predefined = PascalPredefined;

        return value switch
        {
            int => predefined.IntegerType,
            double or float => predefined.RealType,
            string { Length: 1 } => predefined.CharType,
            string stringValue => PascalTypeFactory.CreateStringType(stringValue, predefined),
            _ => predefined.UndefinedType
        };
    }

    /// <summary>
    /// Gets the type of a constant from its identifier.
    /// </summary>
    /// <param name="identifier">The constant's identifier</param>
    /// <returns>The constant's type or the predefined undefined type when it cannot be resolved.</returns>
    private ITypeSpec GetConstantType(Token identifier)
    {
        if (identifier.Text is null)
            return PascalPredefined.UndefinedType;

        ISymbolTableEntry? id = SymbolTableStack.Lookup(identifier.Text);

        if (id == null)
            return PascalPredefined.UndefinedType;

        if (id[PascalSymbolTableKey.Definition] is not Definition definition)
            throw new InvalidOperationException("The symbol has no Pascal definition.");

        if (definition == Definition.Constant || definition == Definition.EnumerationConstant)
            return id[PascalSymbolTableKey.TypeSpec] as ITypeSpec ?? PascalPredefined.UndefinedType;

        return PascalPredefined.UndefinedType;
    }

    /// <summary>
    /// Parses a constant referenced by identifier.
    /// </summary>
    /// <param name="token">The current token.</param>
    /// <param name="sign">The sign, if any</param>
    /// <returns>The constant value, or <see langword="null"/> when the identifier is invalid.</returns>
    private object? ParseIdentifierConstant(Token token, ITokenType? sign)
    {
        string? name = token.Text;

        if (name is null)
            return null;

        ISymbolTableEntry? id = SymbolTableStack.Lookup(name);

        // consume the identifier
        _ = NextToken();

        // The identifier must already been defined as a constant identifier.
        if (id == null)
        {
            ErrorHandler.Flag(token, PascalErrorCode.IdentifierUndefined, this);
            return null;
        }

        if (id[PascalSymbolTableKey.Definition] is not Definition definition)
            throw new InvalidOperationException("The symbol has no Pascal definition.");

        if (definition == Definition.Constant)
        {
            object? value = id[SymbolTableKey.ConstantValue];
            id.AppendLineNumber(token.LineNumber);

            if (value is int intValue)
            {
                return ApplySign(intValue, sign);
            }

            else if (value is float floatValue)
            {
                return ApplySign(floatValue, sign);
            }

            else if (value is string)
            {
                if (sign != null)
                    ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);

                return value;
            }

            else
            {
                return null;
            }
        }

        else if (definition == Definition.EnumerationConstant)
        {
            object? value = id[SymbolTableKey.ConstantValue];
            id.AppendLineNumber(token.LineNumber);

            if (sign != null)
                ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);

            return value;
        }

        else if (definition == null)
        {
            ErrorHandler.Flag(token, PascalErrorCode.NotConstantIdentifier, this);
            return null;
        }

        else
        {
            ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);
            return null;
        }
    }

    /// <summary>
    /// Parses a constant value.
    /// </summary>
    /// <param name="token">The current token.</param>
    /// <returns>The constant value, or <see langword="null"/> when parsing reports an error.</returns>
    internal object? ParseConstant(Token token)
    {
        ITokenType? sign = null;

        // Synchronize at the start of a constant.
        token = Synchronize(ConstantStartSet);
        ITokenType? tokenType = token.Type;

        // Plus or minus sign?
        if (tokenType == PascalTokenType.Plus || tokenType == PascalTokenType.Minus)
        {
            sign = tokenType;
            token = NextToken(); // consume sign
        }

        // Parse the constant.
        if (token.Type == PascalTokenType.Identifier)
            return ParseIdentifierConstant(token, sign);

        if (token.Type == PascalTokenType.Integer || token.Type == PascalTokenType.Real)
            return ParseNumericConstant(token, sign);

        if (token.Type == PascalTokenType.String)
        {
            if (sign != null)
                ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);

            _ = NextToken();

            return token.Value is string value
                ? value
                : FlagInvalidConstant(token);
        }

        return FlagInvalidConstant(token);
    }

    private object? ParseNumericConstant(Token token, ITokenType? sign)
    {
        object? value = token.Value;
        _ = NextToken(); // consume the number

        return value switch
        {
            int integerValue => ApplySign(integerValue, sign),
            double realValue => ApplySign(realValue, sign),
            _ => FlagInvalidConstant(token)
        };
    }

    private object? FlagInvalidConstant(Token token)
    {
        ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);

        return null;
    }

    private static T ApplySign<T>(T value, ITokenType? sign) where T : INumber<T>
    {
        return sign == PascalTokenType.Minus
            ? -value
            : value;
    }
}
