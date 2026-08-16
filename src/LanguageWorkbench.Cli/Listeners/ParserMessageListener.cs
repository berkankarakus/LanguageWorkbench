using System.Text;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Messaging;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Writes parser diagnostics and summary information to the console.
/// </summary>
public sealed class ParserMessageListener : IMessageListener
{
    private const string TokenFormat = ">>> {0,-15} line={1:D3}, pos={2,2}, text=\"{3}\"";
    private const string ValueFormat = ">>>                 value={0}";
    private const int PrefixWidth = 5;

    /// <summary>
    /// Called by the parser whenever it produces a message.
    /// </summary>
    /// <param name="message"></param>
    public void MessageReceived(Message message)
    {
        if (message.Body is not object[] body)
            return;

        switch (message.Type)
        {
            case MessageType.ParserSummary
                when body is [int statementCount, int syntaxErrorCount, float elapsedTime]:

                Console.WriteLine(
                    $"\n{statementCount,20} source statements.\n" +
                    $"{syntaxErrorCount,20} syntax errors.\n" +
                    $"{elapsedTime,20:F2} seconds total parsing time.\n"
                );
                break;

            case MessageType.Token when body is [int line, int position, ITokenType tokenType, string text, var value]:
                Console.WriteLine(TokenFormat, tokenType, line, position, text);

                if (value != null && tokenType == PascalTokenType.String)
                    value = $"\"{value}\"";

                Console.WriteLine(ValueFormat, value);
                break;

            case MessageType.SyntaxError
                when body is [_, int position, var tokenText, string errorMessage]:

                int spaceCount = PrefixWidth + position;
                StringBuilder flagBuffer = new();

                for (int i = 1; i < spaceCount; ++i)
                    flagBuffer.Append(' ');

                // A pointer to the error followed by the error message.
                flagBuffer.Append("^\n*** ").AppendLine(errorMessage);

                // Text, if any. of the bad token.
                if (tokenText is not null)
                    flagBuffer.Append(" [at \"").Append(tokenText).Append("\"]");

                Console.WriteLine(flagBuffer.ToString());
                break;
        }
    }
}
