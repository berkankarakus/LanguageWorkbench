using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Messaging;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Writes parser diagnostics and summary information to the console.
/// </summary>
/// <param name="showTokens">Whether token information should be written.</param>
/// <param name="useColor">Whether terminal colors should be used for diagnostics.</param>
public sealed class ParserMessageListener(bool showTokens, bool useColor = false) : IMessageListener
{
    private const int PrefixWidth = 5;
    private readonly bool _useColor = useColor;
    private readonly List<string> _pendingTokenLines = [];

    /// <summary>
    /// Writes the token lines accumulated for the current source line.
    /// </summary>
    public void FlushTokens()
    {
        foreach (string tokenLine in _pendingTokenLines)
            Console.WriteLine(tokenLine);

        _pendingTokenLines.Clear();
    }

    /// <summary>
    /// Called by the parser whenever it produces a message.
    /// </summary>
    /// <param name="message">The parser message to render.</param>
    public void MessageReceived(Message message)
    {
        if (message.Body is not object?[] body)
            return;

        switch (message.Type)
        {
            case MessageType.ParserSummary when body is [int statementCount, int syntaxErrorCount, float elapsedTime]:
                FlushTokens();
                Console.WriteLine(
                    $"\n{statementCount,10} source statements.\n" +
                    $"{syntaxErrorCount,10} syntax errors.\n" +
                    $"{elapsedTime,10:F2} seconds total parsing time."
                );

                break;

            case MessageType.Token when showTokens && body is [int line, int position, ITokenType tokenType, string text, var value]:
                _pendingTokenLines.Add($"  [Token] {line:D3}:{position + 1:D2}  {tokenType}: \"{text}\"");

                if (value is not null)
                {
                    string formattedValue = tokenType == PascalTokenType.String
                        ? $"\"{value}\""
                        : value.ToString() ?? string.Empty;

                    _pendingTokenLines.Add($"          value: {formattedValue}");
                }

                break;

            case MessageType.SyntaxError
                when body is [_, int position, var tokenText, string errorMessage]:
                int spaceCount = PrefixWidth + position;
                string indentation = new(' ', spaceCount - 1);

                WriteColored(indentation + "^", ConsoleColor.Red);
                Console.WriteLine();

                WriteColored("error: " + errorMessage, ConsoleColor.Red);

                // Text, if any, of the bad token.
                if (tokenText is string tokenTextValue && !string.IsNullOrWhiteSpace(tokenTextValue))
                {
                    Console.Write(" [at \"");
                    WriteColored(tokenTextValue, ConsoleColor.Yellow);
                    Console.Write("\"]");
                }

                Console.WriteLine();
                FlushTokens();
                break;
        }
    }

    private void WriteColored(string text, ConsoleColor color)
    {
        if (!_useColor)
        {
            Console.Write(text);
            return;
        }

        ConsoleColor previousColor = Console.ForegroundColor;

        try
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }
}
