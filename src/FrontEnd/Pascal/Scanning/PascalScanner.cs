using FrontEnd.Input;
using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Scanning;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Scanning;

/// <summary>
/// The Pascal scanner.
/// </summary>
public class PascalScanner(SourceReader source) : Scanner(source)
{
    /// <summary>
    /// Extract and return the next Pascal token from the source.
    /// </summary>
    protected override Token ExtractToken()
    {
        SkipWhitespace();
        char currentChar = CurrentChar();

        if (currentChar == SourceReader.EndOfFile)
            return new EofToken(Source, PascalTokenType.EndOfFile);

        if (char.IsLetter(currentChar))
            return new PascalWordToken(Source);

        if (char.IsDigit(currentChar))
            return new PascalNumberToken(Source);

        if (currentChar == '\'')
            return new PascalStringToken(Source);

        if (PascalTokenType.SpecialSymbols.ContainsKey(currentChar.ToString()))
            return new PascalSpecialSymbolToken(Source);

        Token token = new PascalErrorToken(Source, PascalErrorCode.InvalidCharacter, currentChar.ToString());
        NextChar();

        return token;
    }

    private void SkipWhitespace()
    {
        char currentChar = CurrentChar();

        while (char.IsWhiteSpace(currentChar) || currentChar == '{')
        {
            if (currentChar != '{')
            {
                currentChar = NextChar();
                continue;
            }

            do
            {
                currentChar = NextChar();
            } while (currentChar is not '}' and not SourceReader.EndOfFile);

            if (currentChar == '}')
                currentChar = NextChar();
        }
    }
}
