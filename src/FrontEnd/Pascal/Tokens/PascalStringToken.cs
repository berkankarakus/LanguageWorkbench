using System.Text;
using FrontEnd.Input;
using FrontEnd.Pascal.ErrorHandling;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Represents a Pascal string literal token.
/// </summary>
public class PascalStringToken(SourceReader source) : PascalToken(source)
{
    private const char Quote = '\'';

    /// <inheritdoc />
    protected override void Extract()
    {
        StringBuilder textBuffer = new();
        StringBuilder valueBuffer = new();

        char currentChar = NextChar(); // consume initial quote
        textBuffer.Append(Quote);

        // Get string characters.
        do
        {
            // Replace any whitespace character with a blank.
            if (char.IsWhiteSpace(currentChar))
                currentChar = ' ';

            if (currentChar != Quote && currentChar != SourceReader.EndOfFile)
            {
                textBuffer.Append(currentChar);
                valueBuffer.Append(currentChar);
                currentChar = NextChar();
            }

            if (currentChar != Quote)
                continue;

            while (currentChar == Quote && PeekChar() == Quote)
            {
                textBuffer.Append(Quote).Append(Quote);
                valueBuffer.Append(currentChar); // append single quote
                _ = NextChar(); // consume a pair of quotes
                currentChar = NextChar();
            }
        } while (currentChar != Quote && currentChar != SourceReader.EndOfFile);

        if (currentChar == Quote)
        {
            _ = NextChar(); // consume final quote
            textBuffer.Append(Quote);

            Type = PascalTokenType.String;
            Value = valueBuffer.ToString();
        }

        else
        {
            Type = PascalTokenType.Error;
            Value = PascalErrorCode.UnexpectedToken;
        }

        Text = textBuffer.ToString();
    }
}
