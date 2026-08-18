using System.Text;
using FrontEnd.Input;

namespace FrontEnd.Pascal.Tokens;

public class PascalWordToken(SourceReader source) : PascalToken(source)
{
    /// <summary>
    /// Extract a Pascal word token from a source.
    /// </summary>
    protected override void Extract()
    {
        StringBuilder textBuffer = new();
        char currentChar = CurrentChar();

        while (char.IsLetterOrDigit(currentChar))
        {
            textBuffer.Append(currentChar);
            currentChar = NextChar();
        }

        Text = textBuffer.ToString();

        // is it a reserved word or an identifier?
        Type = PascalTokenType.ReservedWords.Contains(Text)
            ? PascalTokenType.ReservedWord
            : PascalTokenType.Identifier;
    }
}