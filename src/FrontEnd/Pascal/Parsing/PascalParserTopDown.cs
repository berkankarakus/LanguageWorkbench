using System.Diagnostics;
using FrontEnd.Parsing;
using FrontEnd.Scanning;
using FrontEnd.Tokens;
using Messaging;

namespace FrontEnd.Pascal.Parsing;

/// <summary>
/// The top-down Pascal parser.
/// </summary>
/// <param name="scanner"></param>
public class PascalParserTopDown(Scanner scanner) : Parser(scanner)
{
    public override void Parse()
    {
        Token token;
        long startTime = Stopwatch.GetTimestamp();

        do
        {
            token = NextToken();
        } while (token is not EofToken);

        // Send the parser summary message.
        float elapsedTime = (float)Stopwatch.GetElapsedTime(startTime).TotalSeconds;
        SendMessage(new(MessageType.ParserSummary, new object[] { token.LineNumber, GetErrorCount(), elapsedTime }));
    }

    /// <summary>
    /// Return the number of syntax errors found by the parser.
    /// </summary>
    /// <returns></returns>
    public override int GetErrorCount()
    {
        return 0;
    }
}
