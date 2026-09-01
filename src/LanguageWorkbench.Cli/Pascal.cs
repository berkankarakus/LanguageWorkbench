using Backend;
using FrontEnd.Input;
using FrontEnd.Parsing;
using FrontEnd.Pascal.Factory;
using FrontEnd.Pascal.Parsing;
using Intermediate.Code.Interfaces;
using Intermediate.Code.Utilities;
using Intermediate.Symbols.Interfaces;
using Intermediate.Symbols.Utilities;
using LanguageWorkbench.Cli.Listeners;

namespace LanguageWorkbench.Cli;

public class Pascal
{
    private const string ShowTokensEnvironmentVariable = "LANGUAGE_WORKBENCH_SHOW_TOKENS";
    private readonly string _operation;
    private readonly string _filePath;
    private readonly bool _crossReference;
    private readonly bool _intermediate;

    /// <summary>
    /// Configures a Pascal translation request.
    /// </summary>
    /// <param name="operation">The requested compiler operation.</param>
    /// <param name="filePath">The path to the Pascal source file.</param>
    /// <param name="flags">The compiler flags.</param>
    private Pascal(string operation, string filePath, string flags)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(flags);

        if (flags.Any(static flag => flag is not ('i' or 'x')))
            throw new ArgumentException("Flags must contain only 'i' and 'x'.", nameof(flags));

        _operation = operation;
        _filePath = filePath;
        _crossReference = flags.Contains('x');
        _intermediate = flags.Contains('i');
    }

    /// <summary>
    /// Compiles or interprets a Pascal source program.
    /// </summary>
    /// <param name="operation">The requested compiler operation.</param>
    /// <param name="filePath">The path to the Pascal source file.</param>
    /// <param name="flags">The compiler flags.</param>
    public static void Run(string operation, string filePath, string flags)
    {
        new Pascal(operation, filePath, flags).Translate();
    }

    private void Translate()
    {
        try
        {
            using var source = new SourceReader(File.OpenText(_filePath));
            ParserMessageListener parserMessageListener = new(ShouldShowTokens());
            source.AddMessageListener(new SourceMessageListener(parserMessageListener.FlushTokens));

            PascalFrontEndFactory<PascalParserTopDown> factory = new(scanner => new PascalParserTopDown(scanner));
            Parser parser = factory.CreateParser(source);
            parser.AddMessageListener(parserMessageListener);

            BackendProcessor backEnd = BackendFactory.Create(_operation);
            backEnd.AddMessageListener(new BackendMessageListener());

            parser.Parse();

            if (parser.ErrorCount > 0)
                return;

            IIntermediateCode iCode = parser.Code;
            ISymbolTableStack symbolTableStack = Parser.SymbolTableStack;

            if (_crossReference)
                CrossReferencer.Print(symbolTableStack);

            if (_intermediate)
            {
                ParseTreePrinter parseTreePrinter = new(Console.Out);
                parseTreePrinter.Print(iCode);
            }

            backEnd.Process(iCode, symbolTableStack);
        }

        catch (Exception e)
        {
            Console.WriteLine("***** Internal translator error. *****");
            Console.WriteLine(e.StackTrace);

            throw;
        }
    }

    private static bool ShouldShowTokens()
    {
        string? value = Environment.GetEnvironmentVariable(ShowTokensEnvironmentVariable);
        return value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
