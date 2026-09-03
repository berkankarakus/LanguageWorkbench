using System.Diagnostics;
using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Parsing;
using FrontEnd.Scanning;
using FrontEnd.Tokens;
using Intermediate.Code.Interfaces;
using Messaging;

namespace FrontEnd.Pascal.Parsing.BottomUp;

/// <summary>
/// Parses the currently supported Pascal statement subset with shift-reduce operations.
/// </summary>
/// <remarks>
/// Compound statements are reduced when their <c>END</c> token is encountered. Expression
/// operands and operators are reduced with precedence-aware stacks.
/// </remarks>
/// <param name="scanner">The scanner that supplies Pascal tokens.</param>
public class PascalParser(Scanner scanner) : Parser(scanner)
{
    /// <summary>
    /// Handles syntax errors reported while parsing Pascal source.
    /// </summary>
    protected static readonly PascalErrorHandler ErrorHandler = new();

    /// <summary>
    /// Creates a parser that uses the scanner of an existing bottom-up parser.
    /// </summary>
    /// <param name="parent">The parser whose scanner will be reused.</param>
    protected PascalParser(PascalParser parent) : this(parent.Scanner) { }

    /// <inheritdoc />
    public override int ErrorCount => ErrorHandler.ErrorCount;

    /// <inheritdoc />
    public override void Parse()
    {
        long startTime = Stopwatch.GetTimestamp();
        Token token = NextToken();

        try
        {
            CompoundStatementParser compoundParser = new(this);
            IIntermediateCodeNode rootNode = compoundParser.Parse(ref token);

            if (token.Type != PascalTokenType.Dot)
                ErrorHandler.Flag(token, PascalErrorCode.MissingPeriod, this);

            Code.Root = rootNode;
            _ = NextToken();
            SendSummary(token.LineNumber, startTime);
        }

        catch (PascalSyntaxException)
        {
            // The syntax error was already reported by PascalErrorHandler.
        }

        catch (Exception)
        {
            PascalErrorHandler.AbortTranslation(PascalErrorCode.IoError, parser: this);
        }
    }

    /// <summary>
    /// Sends the parser summary for the completed translation.
    /// </summary>
    /// <param name="lineNumber">The line number of the final token.</param>
    /// <param name="startTime">The timestamp recorded before parsing began.</param>
    private void SendSummary(int lineNumber, long startTime)
    {
        float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;

        SendMessage(new Message(
            MessageType.ParserSummary,
            body: new object?[]
            {
                lineNumber,
                ErrorCount,
                elapsedTime
            }
        ));
    }
}
