using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Symbols.Implementations;

/// <summary>
/// Identifies Pascal-specific attributes stored on a symbol-table entry.
/// </summary>
/// <param name="Name">The stable name of the Pascal symbol attribute.</param>
public sealed record PascalSymbolTableKey(string Name) : ISymbolTableKey
{
    /// <summary>Identifies how a Pascal symbol is defined.</summary>
    public static PascalSymbolTableKey Definition { get; } = new(nameof(Definition));

    /// <summary>Identifies the Pascal type specification associated with a symbol.</summary>
    public static PascalSymbolTableKey TypeSpec { get; } = new(nameof(TypeSpec));

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
