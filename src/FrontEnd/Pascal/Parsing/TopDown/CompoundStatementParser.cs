using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal compound statements.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public sealed class CompoundStatementParser(PascalParserTopDown parent) : StatementParser(parent)
{
    /// <summary>
    /// Parses a compound statement.
    /// </summary>
    /// <param name="token">The <c>BEGIN</c> token that starts the statement.</param>
    /// <returns>The intermediate-code node for the compound statement.</returns>
    public override IIntermediateCodeNode Parse(Token token)
    {
        token = NextToken();
        var compoundNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Compound);

        // Parse the statement list terminated by the END token.
        StatementParser statementParser = new(parent: this);
        statementParser.ParseList(token, compoundNode, PascalTokenType.End, PascalErrorCode.MissingEnd);

        return compoundNode;
    }
}
