namespace TurboPascal;

/// <summary>
/// Application entry point for TurboPascal.
/// </summary>
internal static class Program
{
    private const string Flags = "[-ix]";
    private const string Usage = "Usage: Pascal execute|compile " + Flags + " <source file path>";

    /// <summary>
    /// Opens a Turbo Pascal source file and compiles/executes.
    /// </summary>
    /// <param name="args">A single source-file path, or <c>--help</c>.</param>
    /// <returns>An application exit code.</returns>
    public static void Main(string[] args)
    {
        try
        {
            string operation = args.Length > 0
                ? args[0]
                : "";

            // operation
            if (operation is not ("compile" or "execute"))
                throw new ArgumentException(Usage);

            int i = 0;
            string flags = "";

            // flags
            while(++i < args.Length && args[i][0] == '-')
                flags += args[i][1];

            // source path
            if (i >= args.Length)
                throw new ArgumentException(Usage);

            string path = args[i];
            Pascal.Run(operation, path, flags);
        }

        catch (Exception)
        {
            Console.WriteLine(Usage);
        }
    }
}
