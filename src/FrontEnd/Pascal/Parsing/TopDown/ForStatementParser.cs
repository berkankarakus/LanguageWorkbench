using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal <c>FOR</c> statements and lowers them to initialization, loop test, body, and update nodes.
/// </summary>
/// <param name="parent">The parser that provides scanner and error-handler services.</param>
public sealed class ForStatementParser(PascalParserTopDown parent) : StatementParser(parent)
{
    // Synchronization set for TO or DOWNTO.
    private static readonly HashSet<ITokenType> ToDownToSet =
    [
        ..ExpressionParser.ExpressionStartSet,
        PascalTokenType.To,
        PascalTokenType.DownTo,
        ..StatementFollowSet
    ];

    // Synchronization set for DO.
    private static readonly HashSet<ITokenType> DoSet =
    [
        ..StatementStartSet,
        PascalTokenType.Do,
        ..StatementFollowSet
    ];

    /// <summary>
    /// Parses a <c>FOR</c> statement beginning at its <c>FOR</c> token.
    /// </summary>
    /// <param name="token">The current <c>FOR</c> token.</param>
    /// <returns>The root <see cref="IntermediateCodeNodeType.Compound"/> node.</returns>
    public override IIntermediateCodeNode Parse(Token token)
    {
        // Consume the FOR.
        token = NextToken();
        Token targetToken = token;

        // Create the loop COMPOUND, LOOP, and TEST nodes.
        var compoundNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Compound);
        var loopNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Loop);
        var testNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Test);

        // Parse the embedded initial assignment.
        AssignmentStatementParser assignmentStatementParser = new(this);
        IIntermediateCodeNode initAssignNode = assignmentStatementParser.Parse(token);

        // Set the current line number attribute.
        SetLineNumber(initAssignNode, targetToken);

        // The COMPOUND node adopts the initial assign and the LOOP nodes.
        compoundNode.AddChildren([initAssignNode, loopNode]);

        // Synchronize at the TO or DOWNTO.
        token = Synchronize(ToDownToSet);
        ITokenType? direction = token.Type;

        // Look for the TO or DOWNTO.
        if (direction == PascalTokenType.To || direction == PascalTokenType.DownTo)
        {
            token = NextToken();
        }
        else
        {
            direction = PascalTokenType.To;
            ErrorHandler.Flag(token, PascalErrorCode.MissingToDownTo, this);
        }

        // Create a relational operator node: GT for TO, or LT for DOWNTO.
        var relOpNode = IntermediateCodeFactory.CreateIntermediateCodeNode(
            direction == PascalTokenType.To
                ? IntermediateCodeNodeType.GreaterThan
                : IntermediateCodeNodeType.LessThan
        );

        /*
         * Copy the control VARIABLE node. The relational operator node adopts
         * the copied VARIABLE node as its first child.
         */
        var controlVarNode = initAssignNode.Children[0];
        relOpNode.AddChild(controlVarNode.Copy());

        /*
         * Parse the termination expression. The relational operator node
         * adopts the expression as its second child.
        */
        ExpressionParser expressionParser = new(this);

        if (token is not EofToken)
        {
            var expressionNode = expressionParser.Parse(token);
            relOpNode.AddChild(expressionNode);
        }

        /*
         * The TEST node adopts the relational operator node as its only child.
         * The LOOP node adopts the TEST node as its first child.
         */
        testNode.AddChild(relOpNode);
        loopNode.AddChild(testNode);

        // Synchronize at the DO.
        token = Synchronize(DoSet);

        if (token.Type == PascalTokenType.Do)
            token = NextToken(); // Consume the DO.
        else
            ErrorHandler.Flag(token, PascalErrorCode.MissingDo, this);

        /*
         * Parse the nested statement. The LOOP node adopts the statement node
         * as its second child.
         */
        StatementParser statementParser = new(this);

        if (token is not EofToken)
            loopNode.AddChild(statementParser.Parse(token));

        /*
         * Create an assignment with a copy of the control variable to advance
         * the value of the variable.
         */
        var nextAssignNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Assignment);
        nextAssignNode.AddChild(controlVarNode.Copy());

        /*
         * Create the arithmetic operator node:
         * ADD for TO, or SUBTRACT for DOWNTO.
         */
        var arithOpNode = IntermediateCodeFactory.CreateIntermediateCodeNode(
            direction == PascalTokenType.To
                ? IntermediateCodeNodeType.Add
                : IntermediateCodeNodeType.Subtract
        );

        /*
         * The operator node adopts a copy of the loop variable as its first
         * child and the value 1 as its second child.
         */
        IIntermediateCodeNode oneNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.IntegerConstant);
        oneNode[IntermediateCodeKey.Value] = 1;
        arithOpNode.AddChildren([controlVarNode.Copy(), oneNode]);

        /*
         * The next ASSIGN node adopts a the arithmetic operator as its second
         * child. The loop node adopts the next ASSIGN node as its third child.
         */
        nextAssignNode.AddChild(arithOpNode);
        loopNode.AddChild(nextAssignNode);

        // Set the current line number attribute.
        SetLineNumber(nextAssignNode, targetToken);

        return compoundNode;
    }

    /// <summary>
    /// Assigns the source line of the loop-control variable to a generated assignment node.
    /// </summary>
    /// <param name="node">The generated assignment node.</param>
    /// <param name="targetToken">The source token for the loop-control variable.</param>
    private static void SetLineNumber(IIntermediateCodeNode node, Token targetToken)
    {
        node[IntermediateCodeKey.Line] = targetToken.LineNumber;
    }
}
