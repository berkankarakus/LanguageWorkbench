using Intermediate.Symbols.Interfaces;

namespace Intermediate.Symbols.Utilities;

/// <summary>
/// Produces a cross-reference table from symbol tables.
/// </summary>
public static class CrossReferencer
{
    private const int NameWidth = 16;
    private const string NumbersLabel = " Line Numbers";
    private const string NumbersUnderline = " ------------";

    /// <summary>
    /// Writes the cross-reference table for the current local scope to standard output.
    /// </summary>
    /// <param name="symbolTableStack">The stack that supplies the local symbol table.</param>
    public static void Print(ISymbolTableStack symbolTableStack)
    {
        ArgumentNullException.ThrowIfNull(symbolTableStack);

        Console.WriteLine("\n===== CROSS-REFERENCE TABLE =====");
        PrintColumnHeadings();
        PrintSymbolTable(symbolTableStack.LocalSymbolTable);
    }

    /// <summary>
    /// Writes the table's column headings.
    /// </summary>
    private static void PrintColumnHeadings()
    {
        Console.WriteLine($"{"Identifier",-NameWidth}{NumbersLabel}");
        Console.WriteLine($"{"----------",-NameWidth}{NumbersUnderline}");
    }

    /// <summary>
    /// Writes the entries contained in a symbol table.
    /// </summary>
    /// <param name="symbolTable">The symbol table whose entries will be written.</param>
    private static void PrintSymbolTable(ISymbolTable symbolTable)
    {
        foreach (ISymbolTableEntry entry in symbolTable.SortedEntries())
        {
            Console.Write($"{entry.Name,-NameWidth}");

            foreach (int lineNumber in entry.LineNumbers)
                Console.Write($" {lineNumber:D3}");

            Console.WriteLine();
        }
    }
}
