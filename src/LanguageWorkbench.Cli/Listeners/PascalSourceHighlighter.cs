using FrontEnd.Pascal.Tokens;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Writes Pascal source lines with lightweight syntax highlighting for a terminal.
/// </summary>
/// <param name="useColor">Whether terminal colors should be applied.</param>
public sealed class PascalSourceHighlighter(bool useColor)
{
    private readonly bool _useColor = useColor;
    private bool _inComment;
    private bool _inString;

    /// <summary>
    /// Writes a numbered source line, highlighting Pascal lexical elements.
    /// </summary>
    /// <param name="lineNumber">The one-based source-line number.</param>
    /// <param name="lineText">The source text to write.</param>
    public void WriteLine(int lineNumber, string lineText)
    {
        Console.Write($"{lineNumber,3:D3} ");

        int index = 0;

        while (index < lineText.Length)
        {
            if (_inComment || lineText[index] == '{')
            {
                index = WriteComment(lineText, index);
                continue;
            }

            if (_inString || lineText[index] == '\'')
            {
                index = WriteString(lineText, index);
                continue;
            }

            if (char.IsLetter(lineText[index]))
            {
                index = WriteWord(lineText, index);
                continue;
            }

            if (char.IsDigit(lineText[index]))
            {
                index = WriteNumber(lineText, index);
                continue;
            }

            Console.Write(lineText[index++]);
        }

        Console.WriteLine();
    }

    private int WriteComment(string lineText, int index)
    {
        int start = index;

        if (!_inComment)
        {
            _inComment = true;
            ++index;
        }

        int closingBraceIndex = lineText.IndexOf('}', index);

        if (closingBraceIndex >= 0)
        {
            index = closingBraceIndex + 1;
            _inComment = false;
        }
        else
        {
            index = lineText.Length;
        }

        WriteColored(lineText[start..index], ConsoleColor.Gray);
        return index;
    }

    private int WriteString(string lineText, int index)
    {
        int start = index;

        if (!_inString)
        {
            _inString = true;
            ++index;
        }

        while (index < lineText.Length)
        {
            if (lineText[index++] != '\'')
                continue;

            if (index < lineText.Length && lineText[index] == '\'')
            {
                ++index;
                continue;
            }

            _inString = false;
            break;
        }

        WriteColored(lineText[start..index], ConsoleColor.Green);
        return index;
    }

    private static int FindNumberEnd(string lineText, int index)
    {
        while (index < lineText.Length && char.IsDigit(lineText[index]))
            ++index;

        if (index + 1 < lineText.Length && lineText[index] == '.' && lineText[index + 1] != '.')
        {
            ++index;

            while (index < lineText.Length && char.IsDigit(lineText[index]))
                ++index;
        }

        if (index < lineText.Length && lineText[index] is 'E' or 'e')
        {
            ++index;

            if (index < lineText.Length && lineText[index] is '+' or '-')
                ++index;

            while (index < lineText.Length && char.IsDigit(lineText[index]))
                ++index;
        }

        return index;
    }

    private int WriteNumber(string lineText, int index)
    {
        int start = index;
        index = FindNumberEnd(lineText, index);
        WriteColored(lineText[start..index], ConsoleColor.Magenta);
        return index;
    }

    private int WriteWord(string lineText, int index)
    {
        int start = index;

        while (index < lineText.Length && char.IsLetterOrDigit(lineText[index]))
            ++index;

        string word = lineText[start..index];
        ConsoleColor color = PascalTokenType.ReservedWords.ContainsKey(word)
            ? ConsoleColor.Cyan
            : ConsoleColor.Yellow;

        WriteColored(word, color);
        return index;
    }

    private void WriteColored(string text, ConsoleColor? color)
    {
        if (!_useColor || color is not { } consoleColor)
        {
            Console.Write(text);
            return;
        }

        ConsoleColor previousColor = Console.ForegroundColor;

        try
        {
            Console.ForegroundColor = consoleColor;
            Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }
}
