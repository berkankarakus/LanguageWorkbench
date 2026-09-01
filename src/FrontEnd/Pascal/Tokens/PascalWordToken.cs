using System.Text;
using FrontEnd.Input;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Represents a Pascal identifier or reserved word.
/// </summary>
public class PascalWordToken(SourceReader source) : PascalToken(source)
{
    /// <summary>
    /// Extracts an identifier or reserved word from the source.
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

        // A reserved word retains its specific token type; all other words are identifiers.
        Type = PascalTokenType.ReservedWords.TryGetValue(Text, out PascalTokenType? reservedWord)
            ? reservedWord
            : PascalTokenType.Identifier;
    }
}
