using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal <c>CASE</c> statements and constructs their intermediate-code subtrees.
/// </summary>
/// <param name="parent">The parser that provides scanner and error-handler services.</param>
public sealed class CaseStatementParser(PascalParserTopDown parent) : StatementParser(parent)
{
    // Synchronization set for the first token of a CASE branch constant.
    private static readonly HashSet<ITokenType> ConstantStartSet =
    [
        PascalTokenType.Identifier,
        PascalTokenType.Integer,
        PascalTokenType.Plus,
        PascalTokenType.Minus,
        PascalTokenType.String
    ];

    // Synchronization set used while looking for OF.
    private static readonly HashSet<ITokenType> OfSet =
    [
        .. ConstantStartSet,
        PascalTokenType.Of,
        .. StatementFollowSet
    ];

    private static readonly HashSet<ITokenType> CommaSet =
    [
        .. ConstantStartSet,
        PascalTokenType.Comma,
        PascalTokenType.Colon,
        .. StatementStartSet,
        .. StatementFollowSet
    ];

    /// <summary>
    /// Parses a <c>CASE</c> statement beginning at its <c>CASE</c> token.
    /// </summary>
    /// <param name="token">The current <c>CASE</c> token.</param>
    /// <returns>The root <see cref="IntermediateCodeNodeType.Select"/> node.</returns>
    public override IIntermediateCodeNode Parse(Token token)
    {
        // Consume the CASE.
        token = NextToken();

        // Create a SELECT node.
        var selectNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Select);

        /*
         * Parse the CASE expression.
         * The SELECT node adopts the expression subtree as its first child.
         */
        ExpressionParser expressionParser = new(this);
        selectNode.AddChild(expressionParser.Parse(token));

        //Synchronize at the OF.
        token = Synchronize(OfSet);

        if (token.Type == PascalTokenType.Of)
            token = NextToken(); // Consume the OF.
        else
            ErrorHandler.Flag(token, PascalErrorCode.MissingOf, this);

        // Set of CASE branch constants.
        HashSet<object> constantSet = [];

        /*
         * Loop to parse each CASE branch until the END token or the end of the
         * source file.
         */
        while (token is not EofToken && token.Type != PascalTokenType.End)
        {
            // The SELECT node adopts the CASE branch subtrees.
            selectNode.AddChild(ParseBranch(token, constantSet));

            token = CurrentToken();
            var tokenType = token.Type;

            // Look for the semicolon between CASE branches.
            if (tokenType == PascalTokenType.SemiColon)
                token = NextToken(); // Consume the ;

            // If at the start of the next constant, then missing a semicolon.
            else if (tokenType != null && ConstantStartSet.Contains(tokenType))
                ErrorHandler.Flag(token, PascalErrorCode.MissingSemicolon, this);
        }

        // Look for the END token.
        if (token.Type == PascalTokenType.End)
            _ = NextToken(); // Consume END.
        else
            ErrorHandler.Flag(token, PascalErrorCode.MissingEnd, this);

        return selectNode;
    }

    /// <summary>
    /// Parses one CASE branch: its constants, colon, and associated statement.
    /// </summary>
    /// <param name="token">The first token of the branch.</param>
    /// <param name="constantSet">Constants already used by preceding branches.</param>
    /// <returns>The root <see cref="IntermediateCodeNodeType.SelectBranch"/> node.</returns>
    private IIntermediateCodeNode? ParseBranch(Token token, HashSet<object> constantSet)
    {
        /*
         * Create a SelectBranch node and a SelectConstants node.
         * The SelectBranch node adopts the SelectConstants node as its first child.
         */
        var branchNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.SelectBranch);
        var constantsNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.SelectConstants);

        branchNode.AddChild(constantsNode);

        /*
         * Parse the list of CASE branch statements.
         * The SelectConstants node adopts each constant.
         */
        ParseConstantList(token, constantsNode, constantSet);

        // Look for the : token.
        token = CurrentToken();

        if (token.Type == PascalTokenType.Colon)
            token = NextToken(); // Consume the : token.
        else
            ErrorHandler.Flag(token, PascalErrorCode.MissingColon, this);

        /*
         * Parse the CASE branch statement. The SelectBranch node adopts the
         * statement subtree as its second child.
         */
        StatementParser statementParser = new(this);
        branchNode.AddChild(statementParser.Parse(token));

        return branchNode;
    }

    /// <summary>
    /// Parses the comma-separated constant list that labels a CASE branch.
    /// </summary>
    /// <param name="token">The first token of the constant list.</param>
    /// <param name="constantsNode">The <c>SelectConstants</c> node that receives the constants.</param>
    /// <param name="constantSet">Constants already used by preceding branches.</param>
    private void ParseConstantList(Token token, IIntermediateCodeNode constantsNode, HashSet<object> constantSet)
    {
        // Loop to parse each constant.
        while (token.Type is { } tokenType && ConstantStartSet.Contains(tokenType))
        {
            // The constants list node adopts the constant node.
            constantsNode.AddChild(ParseConstant(token, constantSet));

            // Synchronize at the comma between constants.
            token = Synchronize(CommaSet);

            // Look for the comma.
            if (token.Type == PascalTokenType.Comma)
                token = NextToken(); // Consume the , token.

            // If at the start of the next constant, then missing a comma.
            else if (token.Type is { } nextTokenType && ConstantStartSet.Contains(nextTokenType))
                ErrorHandler.Flag(token, PascalErrorCode.MissingComma, this);
        }
    }

    /// <summary>
    /// Parses one literal CASE branch constant, optionally preceded by a sign.
    /// </summary>
    /// <param name="token">The first token of the constant.</param>
    /// <param name="constantSet">Constants already used by CASE branches.</param>
    /// <returns>The parsed constant node, or <see langword="null"/> after a syntax error.</returns>
    private IIntermediateCodeNode? ParseConstant(Token token, HashSet<object> constantSet)
    {
        ITokenType? sign = null;
        IIntermediateCodeNode? constantNode = null;

        // Synchronize at the start of a constant.
        token = Synchronize(ConstantStartSet);
        ITokenType? tokenType = token.Type;

        // Plus or minus sign?
        if (tokenType != null && (tokenType == PascalTokenType.Plus || tokenType == PascalTokenType.Minus))
        {
            sign = tokenType;
            token = NextToken();
        }

        // Parse the constant.
        if (token.Type == PascalTokenType.Identifier)
            constantNode = ParseIdentifierConstant(token, sign);

        else if (token.Type == PascalTokenType.Integer)
            constantNode = ParseIntegerConstant(token.Text, sign);

        else if (token.Type == PascalTokenType.String)
            constantNode = ParseCharacterConstant(token, sign);

        else
            ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);

        // Check for reused constants.
        if (constantNode != null)
        {
            object? value = constantNode[IntermediateCodeKey.Value];

            if (value != null)
            {
                if (!constantSet.Add(value))
                    ErrorHandler.Flag(token, PascalErrorCode.CaseConstantReused, this);
            }
        }

        // Consume the constant.
        _ = NextToken();

        return constantNode;
    }

    /// <summary>
    /// Parses a single-character string constant for a CASE branch.
    /// </summary>
    /// <param name="token">The string token.</param>
    /// <param name="sign">An optional preceding sign.</param>
    /// <returns>The constant node, or <see langword="null"/> when the constant is invalid.</returns>
    private IIntermediateCodeNode? ParseCharacterConstant(Token token, ITokenType? sign)
    {
        var value = (string?)token.Value;
        IIntermediateCodeNode? constantNode = null;

        if (sign != null)
        {
            ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);
        }
        else
        {
            if (value is { Length: 1 })
            {
                constantNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.StringConstant);
                constantNode[IntermediateCodeKey.Value] = value;
            }

            else
            {
                ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);
            }
        }

        return constantNode;
    }

    /// <summary>
    /// Parses an integer literal CASE constant.
    /// </summary>
    /// <param name="value">The integer literal text.</param>
    /// <param name="sign">An optional preceding sign.</param>
    /// <returns>The integer constant node.</returns>
    private static IIntermediateCodeNode? ParseIntegerConstant(string? value, ITokenType? sign)
    {
        var constantNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.IntegerConstant);
        int intValue = int.TryParse(value, out int result)
            ? result
            : throw new ArgumentNullException(nameof(value));

        if (sign == PascalTokenType.Minus)
            intValue *= -1;

        constantNode[IntermediateCodeKey.Value] = intValue;
        return constantNode;
    }

    /// <summary>
    /// Reports an identifier constant as unsupported until named constants are implemented.
    /// </summary>
    /// <param name="token">The identifier token.</param>
    /// <param name="sign">An optional preceding sign.</param>
    /// <returns>Always <see langword="null"/>.</returns>
    private IIntermediateCodeNode? ParseIdentifierConstant(Token token, ITokenType? sign)
    {
        // Placeholder: Don't allow for now.
        ErrorHandler.Flag(token, PascalErrorCode.InvalidConstant, this);

        return null;
    }
}
