using FrontEnd.Parsing;
using FrontEnd.Scanning;
using FrontEnd.Tokens;
using Messaging;

namespace FrontEnd.Pascal;

/// <summary>
/// The top-down Pascal parser.
/// </summary>
/// <param name="scanner"></param>
public class PascalParserTopDown(Scanner scanner) : Parser(scanner)
{
    public override void Parse()
    {
        Token token;
        long startTime = DateTime.UtcNow.Ticks;

        do
        {
            token = NextToken();
        } while (token is not EofToken);

        // Send the parser summary message.
        float elapsedTime = (DateTime.UtcNow.Ticks - startTime) / 1000f;
        SendMessage(new(MessageType.ParserSummary, new object[token.LineNumber, GetErrorCount(), (int)elapsedTime]));
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
