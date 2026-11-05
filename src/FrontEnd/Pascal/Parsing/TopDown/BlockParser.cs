using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// Parses the statement section of a Pascal routine block.
/// </summary>
/// <param name="parent">The parser that provides the scanner and translation state.</param>
public sealed class BlockParser(PascalParserTopDown parent) : StatementParser(parent)
{
    /// <summary>
    /// Parses the block associated with a routine.
    /// </summary>
    /// <param name="token">The first token of the block.</param>
    /// <param name="routineId">The symbol-table entry for the routine that owns the block.</param>
    /// <returns>The intermediate-code node for the block.</returns>
    public IIntermediateCodeNode? Parse(Token token, ISymbolTableEntry routineId)
    {
        ArgumentNullException.ThrowIfNull(routineId);

        DeclarationsParser declarationParser = new(this);
        StatementParser statementParser = new(this);

        // Parse any declaration.
        declarationParser.Parse(token);
        token = Synchronize(StatementStartSet);

        ITokenType? tokenType = token.Type;
        IIntermediateCodeNode? rootNode = null;

        // Look for the BEGIN token to parse a compound statement.
        if (tokenType == PascalTokenType.Begin)
        {
            rootNode = statementParser.Parse(token);
        }

        // Missing BEGIN: Attempt to parse anyway if possible.
        else
        {
            ErrorHandler.Flag(token, PascalErrorCode.MissingBegin, this);

            if (tokenType != null && StatementStartSet.Contains(tokenType))
            {
                rootNode = IntermediateCodeFactory.CreateIntermediateCodeNode(
                    IntermediateCodeNodeType.Compound
                );

                statementParser.ParseList(
                    token,
                    rootNode,
                    PascalTokenType.End,
                    PascalErrorCode.MissingEnd
                );
            }
        }

        return rootNode;
    }
}
