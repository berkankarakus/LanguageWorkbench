using FrontEnd.Factory;
using FrontEnd.Input;
using FrontEnd.Parsing;
using FrontEnd.Pascal.Scanning;
using FrontEnd.Scanning;

namespace FrontEnd.Pascal.Factory;

/// <summary>
/// Creates Pascal-specific front-end components.
/// </summary>
/// <typeparam name="TParser">The Pascal parser type to create.</typeparam>
public sealed class PascalFrontEndFactory<TParser>(
    Func<Scanner, TParser> parserFactory
) : FrontEndFactory<TParser> where TParser : Parser
{
    private readonly Func<Scanner, TParser> _parserFactory =
        parserFactory ??
        throw new ArgumentNullException(nameof(parserFactory));

    /// <inheritdoc />
    public override TParser CreateParser(SourceReader source)
    {
        Scanner scanner = new PascalScanner(source);
        return _parserFactory(scanner);
    }
}
