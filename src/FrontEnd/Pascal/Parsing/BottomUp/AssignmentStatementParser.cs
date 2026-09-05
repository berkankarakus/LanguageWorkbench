using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Parsing.BottomUp;

/// <summary>
/// Reduces a Pascal assignment statement into an intermediate-code node.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public sealed class AssignmentStatementParser(PascalParserBottomUp parent) : StatementParser(parent)
{
    /// <inheritdoc />
    public override IIntermediateCodeNode Parse(ref Token token)
    {
        ArgumentNullException.ThrowIfNull(token.Text);
        var assignmentNode = IntermediateCodeFactory.CreateIntermediateCodeNode(
            IntermediateCodeNodeType.Assignment
        );
        string name = token.Text.ToLowerInvariant();
        ISymbolTableEntry entry =
            SymbolTableStack.Lookup(name) ?? SymbolTableStack.EnterLocal(name);
        entry.AppendLineNumber(token.LineNumber);

        var variableNode = IntermediateCodeFactory.CreateIntermediateCodeNode(
            IntermediateCodeNodeType.Variable
        );
        variableNode[IntermediateCodeKey.Id] = entry;
        assignmentNode.AddChild(variableNode);
        assignmentNode[IntermediateCodeKey.Line] = token.LineNumber;

        token = NextToken();

        if (token.Type != PascalTokenType.ColonEquals)
            ErrorHandler.Flag(token, PascalErrorCode.MissingColonEquals, parser: this);

        token = NextToken();
        ExpressionParser expressionParser = new(parent: this);
        assignmentNode.AddChild(expressionParser.Parse(ref token));

        return assignmentNode;
    }
}
