using Backend;
using FrontEnd.Input;
using FrontEnd.Parsing;
using FrontEnd.Pascal.Factory;
using FrontEnd.Pascal.Parsing;
using Intermediate.Code;
using Intermediate.Symbols;
using LanguageWorkbench.Cli.Listeners;

namespace LanguageWorkbench.Cli;

public class Pascal
{
    private readonly string _operation;
    private readonly string _filePath;

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
            ParserMessageListener parserMessageListener = new();
            source.AddMessageListener(new SourceMessageListener(parserMessageListener.FlushTokens));

            PascalFrontEndFactory<PascalParserTopDown> factory = new(scanner => new PascalParserTopDown(scanner));
            Parser parser = factory.CreateParser(source);
            parser.AddMessageListener(parserMessageListener);

            BackendProcessor backEnd = BackendFactory.Create(_operation);
            backEnd.AddMessageListener(new BackendMessageListener());

            parser.Parse();
            ICode iCode = parser.Code;
            ISymbolTable symbolTable = Parser.SymbolTable;
            backEnd.Process(iCode, symbolTable);
        }

        catch (Exception e)
        {
            Console.WriteLine("***** Internal translator error. *****");
            Console.WriteLine(e.StackTrace);

            throw;
        }
    }
}
