using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses Pascal statements and dispatches to statement-specific parsers.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public class StatementParser(PascalParserTopDown parent) : PascalParserTopDown(parent)
{
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
            CompoundStatementParser compoundStatementParser = new(parent: this);
            statementNode = compoundStatementParser.Parse(token);
        }

        else if (token.Type == PascalTokenType.Identifier)
        {
            AssignmentStatementParser assignmentStatementParser = new(parent: this);
            statementNode = assignmentStatementParser.Parse(token);
        }

        else
        {
            // Do not return a no-op node without consuming the token. ParseList would then retry
            // the same token indefinitely.
            ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, parser: this);
            return null;
        }

        // Set the current line number as an attribute.
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
        Token? token,
        IIntermediateCodeNode parentNode,
        PascalTokenType terminator,
        PascalErrorCode errorCode
    )
    {
        // Loop to parse each statement until the END token or the end of the source file.
        while (token is not (null or EofToken) && token.Type != terminator)
        {
            // Parse a statement. The parent node adopts the statement code.
            IIntermediateCodeNode? statementNode = Parse(token);
            parentNode.AddChild(statementNode);

            token = CurrentToken();

            if (token?.Type is null)
                continue;

            ITokenType tokenType = token.Type;

            // Look for the semicolon between statements.
            if (tokenType == PascalTokenType.SemiColon)
                token = NextToken(); // consume the semicolon

            // If at the start of the next assingment statement, then missing a semicolon.
            else if (tokenType == PascalTokenType.Identifier)
                ErrorHandler.Flag(token, PascalErrorCode.MissingSemicolon, parser: this);

            // Unexpected token
            else if (tokenType == terminator)
            {
                ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, parser: this);
                token = NextToken(); // consume the unexpected token
            }
        }

        // Look for the terminator token.
        if (token is null)
            return;

        if (token.Type == terminator)
            _ = NextToken();
        else
            ErrorHandler.Flag(token, errorCode, parser: this);
    }

    private static void SetLineNumber(IIntermediateCodeNode statementNode, Token token)
    {
        statementNode[IntermediateCodeKey.Line] = token.LineNumber;
    }
}
