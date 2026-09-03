using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace FrontEnd.Pascal.Parsing.BottomUp;

/// <summary>
/// Reduces nested <c>BEGIN</c> and <c>END</c> token pairs into compound-statement nodes.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public sealed class CompoundStatementParser(PascalParser parent) : StatementParser(parent)
{
    /// <summary>
    /// Parses a compound statement and leaves its trailing period as the current token.
    /// </summary>
    /// <param name="token">The <c>BEGIN</c> token that starts the outer compound statement.</param>
    /// <returns>The reduced outer compound-statement node.</returns>
    public override IIntermediateCodeNode Parse(ref Token token)
    {
        IIntermediateCodeNode? rootNode = null;
        Stack<IIntermediateCodeNode> compoundNodes = new();

        while (token is not EofToken)
        {
            if (token.Type == PascalTokenType.Begin)
            {
                compoundNodes.Push(CreateCompoundNode(token));
                token = NextToken();
            }

            else if (token.Type == PascalTokenType.End)
            {
                if (compoundNodes.Count == 0)
                    ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, parser: this);

                IIntermediateCodeNode compoundNode = compoundNodes.Pop();
                token = NextToken();

                if (compoundNodes.Count == 0)
                {
                    rootNode = compoundNode;
                    break;
                }

                compoundNodes.Peek().AddChild(compoundNode);
                ConsumeStatementTerminator(ref token);
            }

            else
            {
                if (compoundNodes.Count == 0)
                    ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, parser: this);

                StatementParser statementParser = new(parent: this);
                IIntermediateCodeNode statementNode = statementParser.Parse(ref token);
                compoundNodes.Peek().AddChild(statementNode);
                ConsumeStatementTerminator(ref token);
            }
        }

        if (rootNode is null)
            ErrorHandler.Flag(token, PascalErrorCode.MissingEnd, parser: this);

        return rootNode!;
    }

    private static IIntermediateCodeNode CreateCompoundNode(Token token)
    {
        var compoundNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Compound);
        compoundNode[IntermediateCodeKey.Line] = token.LineNumber;
        return compoundNode;
    }
}
