using FrontEnd.Input;
using FrontEnd.Scanning;
using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Scanning;

/// <summary>
/// The Pascal scanner
/// </summary>
/// <param name="source"></param>
public class PascalScanner(SourceReader source) : Scanner(source)
{
    private readonly SourceReader _source = source;

    /// <summary>
    /// Extract and return the next Pascal token from the source.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override Token ExtractToken()
    {
        char currentChar = CurrentChar();

        // Construct the next token. The current character determines the token type.
        return currentChar == SourceReader.EndOfFile
            ? new EofToken(_source)
            : new Token(_source);
    }
}
