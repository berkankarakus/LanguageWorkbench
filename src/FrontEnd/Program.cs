using InputSource = FrontEnd.Source.Source;

namespace FrontEnd;

/// <summary>
/// Application entry point for the TurboPascal front end.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Opens a Turbo Pascal source file and prepares it for lexical scanning.
    /// </summary>
    /// <param name="args">A single source-file path, or <c>--help</c>.</param>
    /// <returns>An application exit code.</returns>
    public static int Main(string[] args)
    {
        if (args is ["--help"] or ["-h"])
        {
            Console.WriteLine("Usage: FrontEnd <source-file>");
            return 0;
        }

        if (args.Length != 1)
        {
            Console.Error.WriteLine("A single Turbo Pascal source-file path is required. Use --help for usage.");
            return 1;
        }

        string sourcePath = args[0];

        if (!File.Exists(sourcePath))
        {
            Console.Error.WriteLine($"Source file not found: {sourcePath}");
            return 1;
        }

        try
        {
            using var source = new InputSource(File.OpenText(sourcePath));
            Console.WriteLine($"Source file ready for scanning: {Path.GetFullPath(sourcePath)}");
            return 0;
        }
        catch (IOException exception)
        {
            Console.Error.WriteLine($"Unable to open source file: {exception.Message}");
            return 1;
        }
    }
}
