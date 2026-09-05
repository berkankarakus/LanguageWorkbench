using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Interfaces;

namespace FrontEnd.Pascal.Parsing.BottomUp;

/// <summary>
/// Dispatches Pascal statements while a bottom-up parser reduces a compound statement.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public class StatementParser(PascalParserBottomUp parent) : PascalParserBottomUp(parent)
{
    /// <summary>
    /// Parses a statement that starts at the current token.
    /// </summary>
    /// <param name="token">The first token of the statement.</param>
    /// <returns>The reduced intermediate-code node.</returns>
    public virtual IIntermediateCodeNode Parse(ref Token token)
    {
        if (token.Type == PascalTokenType.Identifier)
        {
            AssignmentStatementParser assignmentParser = new(this);
            return assignmentParser.Parse(ref token);
        }

        ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, this);
        throw new InvalidOperationException("Unreachable after reporting a syntax error.");
    }

    /// <summary>
    /// Consumes the separator after a statement or reports a missing semicolon.
    /// </summary>
    /// <param name="token">The token following the completed statement.</param>
    protected internal void ConsumeStatementTerminator(ref Token token)
    {
        if (token.Type == PascalTokenType.SemiColon)
        {
            token = NextToken();
            return;
        }

        if (RequiresStatementTerminator(token))
            ErrorHandler.Flag(token, PascalErrorCode.MissingSemicolon, this);
    }

    private static bool RequiresStatementTerminator(Token token)
    {
        return token.Type != PascalTokenType.End && token.Type != PascalTokenType.Dot;
    }
}
