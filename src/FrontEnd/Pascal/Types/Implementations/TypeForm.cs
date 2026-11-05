using FrontEnd.Pascal.Types.Interfaces;

namespace FrontEnd.Pascal.Types.Implementations;

/// <summary>
/// Defines the structural forms used by a Pascal type specification.
/// </summary>
/// <param name="Name">The stable name of the type form.</param>
public sealed record TypeForm(string Name) : ITypeForm
{
    /// <summary>Represents a scalar type.</summary>
    public static TypeForm Scalar { get; } = new(nameof(Scalar));

    /// <summary>Represents an enumeration type.</summary>
    public static TypeForm Enumeration { get; } = new(nameof(Enumeration));

    /// <summary>Represents a subrange type.</summary>
    public static TypeForm Subrange { get; } = new(nameof(Subrange));

    /// <summary>Represents an array type.</summary>
    public static TypeForm Array { get; } = new(nameof(Array));

    /// <summary>Represents a record type.</summary>
    public static TypeForm Record { get; } = new(nameof(Record));

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
