using FrontEnd.Pascal.Types.Interfaces;

namespace FrontEnd.Pascal.Types.Implementations;

/// <summary>
/// Identifies attributes that can be associated with a Pascal type specification.
/// </summary>
/// <param name="Name">The stable name of the type attribute.</param>
public sealed record TypeKey(string Name) : ITypeKey
{
    /// <summary>Identifies enumeration constants.</summary>
    public static TypeKey EnumerationConstants { get; } = new(nameof(EnumerationConstants));

    /// <summary>Identifies the base type of a subrange.</summary>
    public static TypeKey SubrangeBaseType { get; } = new(nameof(SubrangeBaseType));

    /// <summary>Identifies the lower bound of a subrange.</summary>
    public static TypeKey SubrangeMinValue { get; } = new(nameof(SubrangeMinValue));

    /// <summary>Identifies the upper bound of a subrange.</summary>
    public static TypeKey SubrangeMaxValue { get; } = new(nameof(SubrangeMaxValue));

    /// <summary>Identifies an array's index type.</summary>
    public static TypeKey ArrayIndexType { get; } = new(nameof(ArrayIndexType));

    /// <summary>Identifies an array's element type.</summary>
    public static TypeKey ArrayElementType { get; } = new(nameof(ArrayElementType));

    /// <summary>Identifies an array's element count.</summary>
    public static TypeKey ArrayElementCount { get; } = new(nameof(ArrayElementCount));

    /// <summary>Identifies the symbol table of a record type.</summary>
    public static TypeKey RecordSymbolTable { get; } = new(nameof(RecordSymbolTable));

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
