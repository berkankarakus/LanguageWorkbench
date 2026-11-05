using System.Diagnostics;
using FrontEnd.Parsing;
using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Symbols.Implementations;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Pascal.Types.Implementations;
using FrontEnd.Scanning;
using FrontEnd.Tokens;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Implementations;
using Intermediate.Symbols.Interfaces;
using Messaging;

namespace FrontEnd.Pascal.Parsing.TopDown;

/// <summary>
/// The top-down Pascal parser.
/// </summary>
public class PascalParserTopDown : Parser
{
    /// <summary>
    /// Handles syntax errors reported while parsing Pascal source.
    /// </summary>
    protected PascalErrorHandler ErrorHandler => PascalContext.ErrorHandler;

    /// <summary>
    /// Gets the predefined Pascal types and constants for this translation.
    /// </summary>
    protected Predefined PascalPredefined => PascalContext.Predefined;

    private PascalTranslationContext PascalContext => (PascalTranslationContext)TranslationContext;

    /// <summary>
    /// Creates the root top-down parser for one Pascal translation.
    /// </summary>
    /// <param name="scanner">The scanner that supplies Pascal tokens.</param>
    public PascalParserTopDown(Scanner scanner) : base(scanner, new PascalTranslationContext()) { }

    /// <summary>
    /// Creates a parser that uses the scanner of an existing parser.
    /// </summary>
    /// <param name="parent">The parser whose scanner will be reused.</param>
    public PascalParserTopDown(PascalParserTopDown parent) : base(parent) { }

    /// <summary>
    /// Processes Pascal tokens and publishes their diagnostics and parsing summary.
    /// </summary>
    public void ProcessTokens()
    {
        long startTime = Stopwatch.GetTimestamp();

        try
        {
            Token token;

            do
            {
                token = NextToken();
                ITokenType? tokenType = token.Type;

                // Cross-reference only the identifiers.
                if (tokenType == PascalTokenType.Identifier)
                {
                    string? name = token.Text?.ToLowerInvariant();

                    // If it's not already in the symbol table, create and enter a new entry for the identifier.
                    if (!string.IsNullOrEmpty(name))
                    {
                        ISymbolTableEntry entry =
                            SymbolTableStack.Lookup(name) ?? SymbolTableStack.EnterLocal(name);

                        entry.AppendLineNumber(token.LineNumber);
                    }
                }

                float tokenElapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;

                SendMessage(
                    new Message(
                        MessageType.ParserSummary,
                        body: new object?[] { token.LineNumber, ErrorCount, tokenElapsedTime }
                    )
                );

                if (tokenType is null || tokenType == PascalTokenType.Error)
                {
                    if (token.Value is PascalErrorCode errorCode)
                        ErrorHandler.Flag(token, errorCode, parser: this);
                }
                else
                {
                    SendMessage(
                        new Message(
                            MessageType.Token,
                            body: new[]
                            {
                                token.LineNumber,
                                token.Position,
                                tokenType,
                                token.Text,
                                token.Value,
                            }
                        )
                    );
                }
            } while (token is not EofToken);

            // Send the parser summary message.
            float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;

            SendMessage(
                new Message(
                    MessageType.ParserSummary,
                    body: new object?[] { token.LineNumber, ErrorCount, elapsedTime }
                )
            );
        }
        catch (IOException)
        {
            PascalErrorHandler.AbortTranslation(PascalErrorCode.IoError, parser: this);
        }
    }

    /// <summary>
    /// Parses a Pascal source program and generates its intermediate code.
    /// </summary>
    public override void Parse()
    {
        long startTime = Stopwatch.GetTimestamp();
        IIntermediateCode iCode = Code;
        PascalPredefined.Initialize(SymbolTableStack);

        // Create a dummy program identifier symbol table entry.
        ISymbolTableEntry routineId = SymbolTableStack.EnterLocal("dummyprogramname");
        routineId[PascalSymbolTableKey.Definition] = Definition.Program;
        SymbolTableStack.ProgramId = routineId;

        /*
         * Push a new symbol table onto the symbol table stack and set the
         * routine's symbol table and intermediate code.
         */
        routineId[SymbolTableKey.RoutineSymbolTable] = SymbolTableStack.Push();
        routineId[SymbolTableKey.RoutineIntermediateCode] = iCode;

        BlockParser blockParser = new(this);

        try
        {
            Token token = NextToken();

            // Parse a block.
            IIntermediateCodeNode? rootNode = blockParser.Parse(token, routineId);
            iCode.Root = rootNode;
            SymbolTableStack.Pop();

            // Look for the final period.
            token = CurrentToken();

            if (token.Type != PascalTokenType.Dot)
                ErrorHandler.Flag(token, PascalErrorCode.MissingPeriod, this);

            token = CurrentToken();

            // Send the parser summary message.
            float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;

            SendMessage(
                new Message(
                    MessageType.ParserSummary,
                    body: new[] { token.LineNumber, ErrorCount, elapsedTime }
                )
            );
        }

        catch (IOException)
        {
            PascalErrorHandler.AbortTranslation(PascalErrorCode.IoError, parser: this);
        }
    }

    /// <summary>
    /// Gets the number of syntax errors found while parsing.
    /// </summary>
    public override int ErrorCount => ErrorHandler.ErrorCount;

    /// <summary>
    /// Advances past invalid tokens until a token in the synchronization set is found.
    /// </summary>
    /// <param name="synchronizationSet">The token types at which parsing can safely resume.</param>
    /// <returns>The token where the parser has synchronized.</returns>
    public Token Synchronize(ISet<ITokenType> synchronizationSet)
    {
        ArgumentNullException.ThrowIfNull(synchronizationSet);

        Token token = CurrentToken();

        if (token.Type is null || synchronizationSet.Contains(token.Type))
            return token;

        ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, this);

        do
        {
            token = NextToken();
        } while (ShouldContinueSynchronizing(synchronizationSet, token));

        return token;
    }

    private static bool ShouldContinueSynchronizing(
        ISet<ITokenType> synchronizationSet,
        Token token
    )
    {
        if (token is EofToken)
            return false;

        return token.Type is null || !synchronizationSet.Contains(token.Type);
    }
}
