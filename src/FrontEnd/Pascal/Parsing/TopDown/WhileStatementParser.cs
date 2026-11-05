using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal <c>WHILE</c> statements into loop intermediate code.
/// </summary>
/// <param name="parent">The parser that provides scanner and parser state.</param>
public sealed class WhileStatementParser(PascalParserTopDown parent) : StatementParser(parent)
{
    // Synchronization set for DO.
    private static readonly HashSet<ITokenType> DoSet =
    [
        .. StatementStartSet,
        PascalTokenType.Do,
        ..StatementFollowSet
    ];

    /// <summary>
    /// Parses a <c>WHILE</c> statement.
    /// </summary>
    /// <param name="token">The initial <c>WHILE</c> token.</param>
    /// <returns>The root node of the generated intermediate-code tree.</returns>
    public override IIntermediateCodeNode Parse(Token token)
    {
        // Consume the WHILE.
        token = NextToken();

        // Create LOOP, TEST, and NOT nodes.
        var loopNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Loop);
        var breakNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Test);
        var notNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Not);

        // The LOOP node adopts the TEST node as its first child.
        // The TEST node adopts the NOT node as its first child.
        loopNode.AddChild(breakNode);
        breakNode.AddChild(notNode);

        // Parse the expression.
        // The NOT node adopts the expression subtree as its only child.
        ExpressionParser expressionParser = new(this);
        notNode.AddChild(expressionParser.Parse(token));

        // Synchronize at the DO.
        token = Synchronize(DoSet);

        if (token is not EofToken)
        {
            if (token.Type == PascalTokenType.Do)
                token = NextToken(); // Consume the DO token.
            else
                ErrorHandler.Flag(token, PascalErrorCode.MissingDo, this);

            // Parse the statement.
            // The LOOP node adopts the statement subtree as its second child.
            StatementParser statementParser = new(this);
            loopNode.AddChild(statementParser.Parse(token));
        }

        return loopNode;
    }
}
