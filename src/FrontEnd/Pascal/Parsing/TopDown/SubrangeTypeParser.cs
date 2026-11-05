using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Pascal.Types.Factory;
using FrontEnd.Pascal.Types.Implementations;
using FrontEnd.Pascal.Types.Interfaces;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Subrange type parser
/// </summary>
/// <param name="parent"></param>
/// <returns></returns>
public class SubrangeTypeParser(PascalParserTopDown parent) : PascalParserTopDown(parent)
{
    /// <summary>
    /// Parse a Pascal subrange type specification.
    /// </summary>
    /// <param name="token">The current token</param>
    /// <returns>The subrange type specification</returns>
    public ITypeSpec? Parse(Token token)
    {
        ITypeSpec subrangeType = PascalTypeFactory.CreateType(TypeForm.Subrange);

        // Parse the minimum constant.
        Token constantToken = token;
        ConstantDefinitionsParser constantDefinitionsParser = new(this);
        object? minValue = constantDefinitionsParser.ParseConstant(token);

        // Set the minimum constant's type.
        ITypeSpec minType = constantToken.Type == PascalTokenType.Identifier
            ? constantDefinitionsParser.GetConstantType(constantToken)
            : constantDefinitionsParser.GetConstantType(minValue);

        minValue = CheckValueType(constantToken, minValue, minType);

        token = CurrentToken();
        bool sawDotDot = false;

        // Look for the .. token.
        if (token.Type == PascalTokenType.DotDot)
        {
            token = NextToken(); // consume the .. token
            sawDotDot = true;
        }

        if (token.Type is null)
            return null;

        object? maxValue = null;
        ITokenType? tokenType = token.Type;

        // At the start of the maximum constant?
        if (tokenType is null || !ConstantDefinitionsParser.ConstantStartSet.Contains(tokenType))
        {
            ErrorHandler.Flag(constantToken, PascalErrorCode.InvalidSubrangeType, this);
        }

        else
        {
            if (!sawDotDot)
                ErrorHandler.Flag(token, PascalErrorCode.MissingDotDot, this);

            // Parse the maximum constant.
            token = Synchronize(ConstantDefinitionsParser.ConstantStartSet);
            constantToken = token;
            maxValue = constantDefinitionsParser.ParseConstant(token);

            // Set the maximum constant's type.
            ITypeSpec maxType = constantToken.Type == PascalTokenType.Identifier
                ? constantDefinitionsParser.GetConstantType(constantToken)
                : constantDefinitionsParser.GetConstantType(maxValue);

            // Are the min & max value types valid?
            if (minType is null || maxType is null)
                ErrorHandler.Flag(token, PascalErrorCode.IncompatibleTypes, this);

            // Are the min & max value types the same?
            else if (minType != maxType)
                ErrorHandler.Flag(constantToken, PascalErrorCode.InvalidSubrangeType, this);

            // min value > max value?
            else if (minValue is int min && maxValue is int max && min >= max)
                ErrorHandler.Flag(token, PascalErrorCode.MinGtMax, this);
        }

        subrangeType[TypeKey.SubrangeBaseType] = minType;
        subrangeType[TypeKey.SubrangeMinValue] = minValue;
        subrangeType[TypeKey.SubrangeMaxValue] = maxValue;

        return subrangeType;
    }

    /// <summary>
    /// Check a value of a type specification.
    /// </summary>
    /// <param name="token">The current token</param>
    /// <param name="value">The value</param>
    /// <param name="type">The type specification</param>
    /// <returns>The value</returns>
    private object? CheckValueType(Token token, object? value, ITypeSpec type)
    {
        if (type == PascalPredefined.IntegerType)
            return value;

        if (type == PascalPredefined.CharType)
        {
            if (value is string { Length: > 0 } text)
                return char.GetNumericValue(text[0]);

            ErrorHandler.Flag(token, PascalErrorCode.InvalidSubrangeType, this);
            return value;
        }

        if ((TypeForm)type.Form == TypeForm.Enumeration)
            return value;

        ErrorHandler.Flag(token, PascalErrorCode.InvalidSubrangeType, this);
        return value;
    }
}
