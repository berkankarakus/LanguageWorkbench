using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal <c>IF ... THEN ... ELSE</c> statements and constructs their intermediate-code subtrees.
/// </summary>
/// <param name="parent">The parser that provides scanner and error-handler services.</param>
public sealed class IfStatementParser(PascalParserTopDown parent) : StatementParser(parent)
{
    // Synchronization set for THEN.
    private static readonly HashSet<ITokenType> ThenSet =
    [
        ..StatementStartSet,
        PascalTokenType.Then,
        ..StatementFollowSet
    ];


    /// <summary>
    /// Parses an <c>IF</c> statement beginning at its <c>IF</c> token.
    /// </summary>
    /// <param name="token">The current <c>IF</c> token.</param>
    /// <returns>The root <see cref="IntermediateCodeNodeType.If"/> node.</returns>
    public override IIntermediateCodeNode Parse(Token token)
    {
        // Consume the IF.
        token = NextToken();

        // Create an IF node.
        var ifNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.If);

        /*
         * Parse the expression.
         * The IF node adopts the expression as its first child.
         */
        ExpressionParser expressionParser = new(this);
        ifNode.AddChild(expressionParser.Parse(token));

        // Synchronize at the THEN.
        token = Synchronize(ThenSet);

        if (token.Type == PascalTokenType.Then)
            token = NextToken(); // Consume the THEN.
        else
            ErrorHandler.Flag(token, PascalErrorCode.MissingThen, this);

        /*
         * Parse the THEN statement.
         * The IF node adopts the statement subtree as its second child.
         */
        StatementParser statementParser = new(this);
        ifNode.AddChild(statementParser.Parse(token));
        token = CurrentToken();

        // Look for ELSE.
        if (token.Type == PascalTokenType.Else)
        {
            token = NextToken();

            /*
             * Parse the ELSE statement.
             * The IF node adopts the statement subtree as its third child.
             */
            ifNode.AddChild(statementParser.Parse(token));
        }

        return ifNode;
    }
}
