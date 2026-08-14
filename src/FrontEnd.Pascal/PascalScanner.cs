using FrontEnd.Scanning;
using FrontEnd.Tokens;
using PascalSource = FrontEnd.Source.Source;

namespace FrontEnd.Pascal;

/// <summary>
/// The Pascal scanner
/// </summary>
/// <param name="source"></param>
public class PascalScanner(PascalSource source) : Scanner(source)
{
    /// <summary>
    /// Extract and return the next Pascal token from the source.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override Token ExtractToken()
    {
        Token token;
        char currentChar = CurrentChar();

        // Construct the next token. The current character determines the token type.
        if (currentChar == PascalSource.EndOfFile)
            token = new EofToken(source);
        else
            token = new Token(source);

        return token;
    }
}
