using FrontEnd.Input;
using FrontEnd.Pascal.ErrorHandling;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Placeholder for a Pascal special-symbol token.
/// </summary>
public class PascalSpecialSymbolToken(SourceReader source) : PascalToken(source)
{
    /// <summary>
    /// Extract a Pascal special symbol token form the source.
    /// </summary>
    protected override void Extract()
    {
        char currentChar = CurrentChar();
        Text = currentChar.ToString();
        Type = null;

        switch (currentChar)
        {
            // Single-character special symbols.
            case '+':
            case '-':
            case '*':
            case '/':
            case ',':
            case ';':
            case '\'':
            case '=':
            case '(':
            case ')':
            case '[':
            case ']':
            case '{':
            case '}':
            case '^':
                _ = NextChar(); // consume
                break;

            // : or :=
            case ':':
                currentChar = NextChar(); // consume ':'

                if (currentChar == '=')
                {
                    Text += currentChar;
                    _ = NextChar(); // consume '='
                }

                break;

            case '<':
                currentChar = NextChar(); // consume '<'

                if (currentChar is '=' or '>')
                {
                    Text += currentChar;
                    _ = NextChar(); // consume '=' or '>'
                }

                break;

            case '>':
                currentChar = NextChar(); // consume '>'

                if (currentChar is '=')
                {
                    Text += currentChar;
                    _ = NextChar(); // consume '='
                }

                break;

            case '.':
                currentChar = NextChar(); // consume '.'

                if (currentChar is '.')
                {
                    Text += currentChar;
                    _ = NextChar(); // consume '.'
                }

                break;

            default:
                _ = NextChar(); // consume bad character
                Type = PascalTokenType.Error;
                Value = PascalErrorCode.InvalidCharacter;
                break;
        }

        // Set the type if it wasn't an error.
        Type ??= PascalTokenType.SpecialSymbols[Text];
    }
}
