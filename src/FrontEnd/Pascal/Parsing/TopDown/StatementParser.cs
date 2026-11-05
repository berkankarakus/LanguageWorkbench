using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal statements and dispatches to statement-specific parsers.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public class StatementParser(PascalParserTopDown parent) : PascalParserTopDown(parent)
{
    // Synchronization set for starting a statement.
    protected static readonly HashSet<ITokenType> StatementStartSet =
    [
        PascalTokenType.Begin,
        PascalTokenType.Case,
        PascalTokenType.For,
        PascalTokenType.If,
        PascalTokenType.Repeat,
        PascalTokenType.While,
        PascalTokenType.Identifier,
        PascalTokenType.SemiColon,
    ];

    // Synchronization set for following a statement.
    protected internal static readonly HashSet<ITokenType> StatementFollowSet =
    [
        PascalTokenType.SemiColon,
        PascalTokenType.End,
        PascalTokenType.Else,
        PascalTokenType.Until,
        PascalTokenType.Dot,
    ];

    /// <summary>
    /// Parses a statement beginning with the specified token.
    /// </summary>
    /// <param name="token">The first token of the statement.</param>
    /// <returns>The intermediate-code node for the statement.</returns>
    public virtual IIntermediateCodeNode? Parse(Token token)
    {
        IIntermediateCodeNode? statementNode;

        if (token.Type == PascalTokenType.Begin)
        {
            CompoundStatementParser compoundStatementParser = new(this);
            statementNode = compoundStatementParser.Parse(token);
        }

        else if (token.Type == PascalTokenType.Identifier)
        {
            AssignmentStatementParser assignmentParser = new(this);
            statementNode = assignmentParser.Parse(token);
        }

        else if (token.Type == PascalTokenType.Repeat)
        {
            RepeatStatementParser repeatStatementParser = new(this);
            statementNode = repeatStatementParser.Parse(token);
        }

        else if (token.Type == PascalTokenType.While)
        {
            WhileStatementParser whileStatementParser = new(this);
            statementNode = whileStatementParser.Parse(token);
        }

        else if (token.Type == PascalTokenType.For)
        {
            ForStatementParser forStatementParser = new(this);
            statementNode = forStatementParser.Parse(token);
        }

        else if (token.Type == PascalTokenType.If)
        {
            IfStatementParser ifStatementParser = new(this);
            statementNode = ifStatementParser.Parse(token);
        }

        else if (token.Type == PascalTokenType.Case)
        {
            CaseStatementParser caseStatementParser = new(this);
            statementNode = caseStatementParser.Parse(token);
        }

        else
        {
            statementNode = IntermediateCodeFactory.CreateIntermediateCodeNode(
                IntermediateCodeNodeType.NoOp
            );
        }

        // Set the current line as an attribute.
        SetLineNumber(statementNode, token);

        return statementNode;
    }

    /// <summary>
    /// Parses statements until the specified terminating token is reached.
    /// </summary>
    /// <remarks>
    /// This member is available to specialized statement parsers and to parser collaborators in the same assembly.
    /// </remarks>
    /// <param name="token">The first token in the statement list.</param>
    /// <param name="parentNode">The node that receives the parsed statement nodes.</param>
    /// <param name="terminator">The token that ends the statement list.</param>
    /// <param name="errorCode">The error code to report when the terminator is missing.</param>
    protected internal void ParseList(
        Token token,
        IIntermediateCodeNode parentNode,
        PascalTokenType terminator,
        PascalErrorCode errorCode
    )
    {
        HashSet<ITokenType> synchronizationSet = [
            .. StatementStartSet,
            .. StatementFollowSet,
            terminator
        ];

        token = Synchronize(synchronizationSet);

        // Loop to parse each statement until the terminator or the end of the source file.
        while (token is not EofToken && token.Type != terminator)
        {
            // Parse a statement. The parent node adopts the statement code.
            int errorCountBeforeStatement = ErrorHandler.ErrorCount;
            IIntermediateCodeNode? statementNode = Parse(token);
            parentNode.AddChild(statementNode);

            token = CurrentToken();
            bool statementHasError = ErrorHandler.ErrorCount > errorCountBeforeStatement;

            // Look for the semicolon between statements.
            if (token.Type == PascalTokenType.SemiColon)
                _ = NextToken(); // consume the semicolon

            // If at the start of the next statement, then missing a semicolon.
            else if (token.Type is not null && token.Type != terminator && !statementHasError)
                ErrorHandler.Flag(token, PascalErrorCode.MissingSemicolon, this);

            // Synchronize at the start of the next statement.
            token = Synchronize(synchronizationSet);
        }

        // Look for the terminator token.
        if (token.Type == terminator)
            _ = NextToken();

        else
            ErrorHandler.Flag(token, errorCode, parser: this);
    }

    private static void SetLineNumber(IIntermediateCodeNode? statementNode, Token token)
    {
        statementNode?[IntermediateCodeKey.Line] = token.LineNumber;
    }
}
