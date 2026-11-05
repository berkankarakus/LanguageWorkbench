using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal <c>REPEAT ... UNTIL</c> statements and constructs their intermediate-code subtrees.
/// </summary>
/// <param name="parent">The parser that provides scanner and error-handler services.</param>
public sealed class RepeatStatementParser(PascalParserTopDown parent) : StatementParser(parent)
{
    /// <summary>
    /// Parses a <c>REPEAT</c> statement beginning at its <c>REPEAT</c> token.
    /// </summary>
    /// <param name="token">The current <c>REPEAT</c> token.</param>
    /// <returns>The root <see cref="IntermediateCodeNodeType.Loop"/> node.</returns>
    public override IIntermediateCodeNode Parse(Token token)
    {
        // Consume the REPEAT.
        _ = NextToken();

        // Create a LOOP and TEST nodes.
        var loopNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Loop);
        var testNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Test);
        token = CurrentToken();

        // Parse the statement list terminated by the UNTIL token.
        // The LOOP node is the parent of the statement subtrees.
        StatementParser statementParser = new(this);
        statementParser.ParseList(token, loopNode, PascalTokenType.Until, PascalErrorCode.MissingUntil);

        token = CurrentToken();

        if (token is not EofToken)
        {
            // Parse the expression.
            ExpressionParser expressionParser = new(this);
            testNode.AddChild(expressionParser.Parse(token));
            loopNode.AddChild(testNode);
        }

        return loopNode;
    }
}
