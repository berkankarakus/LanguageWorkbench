using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Parsing;

/// <summary>
/// Parses Pascal assignment statements.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public sealed class AssignmentStatementParser(PascalParserTopDown parent) : StatementParser(parent)
{
    /// <summary>
    /// Parses an assignment statement.
    /// </summary>
    /// <param name="token">The identifier that begins the assignment.</param>
    /// <returns>The intermediate-code node for the assignment.</returns>
    public override IIntermediateCodeNode? Parse(Token token)
    {
        ArgumentNullException.ThrowIfNull(token.Text);
        var assignCodeNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Assignment);

        // Look up the target identifier in the symbol table stack.
        // Enter the identifier into the table if it's not found.
        string targetName = token.Text.ToLowerInvariant();
        ISymbolTableEntry targetId = SymbolTableStack.Lookup(targetName) ?? SymbolTableStack.EnterLocal(targetName);

        targetId.AppendLineNumber(token.LineNumber);
        token = NextToken();

        var variableNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Variable);
        variableNode[IntermediateCodeKey.Id] = targetId;

        // The ASSIGN node adops the variable node as its first child.
        assignCodeNode.AddChild(variableNode);

        // Look for the := token.
        if (token.Type == PascalTokenType.ColonEquals)
            token = NextToken();
        else
            ErrorHandler.Flag(token, PascalErrorCode.MissingColonEquals, parser: this);

        // Parse the expression. The ASSIGN node adopts the expression's node at its second child.
        ExpressionParser expressionParser = new(this);
        assignCodeNode.AddChild(expressionParser.Parse(token));

        return assignCodeNode;
    }
}
