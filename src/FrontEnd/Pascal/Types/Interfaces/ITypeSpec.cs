using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Types.Interfaces;

/// <summary>
/// Defines the metadata that describes a Pascal type.
/// </summary>
public interface ITypeSpec
{
    /// <summary>
    /// Gets the structural form of this type specification.
    /// </summary>
    ITypeForm Form { get; }

    /// <summary>
    /// Gets or sets the symbol-table entry that names this type, if it has one.
    /// </summary>
    ISymbolTableEntry? Identifier { get; set; }

    /// <summary>
    /// Gets or sets an attribute associated with this type specification.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    /// <returns>The stored attribute value, if any.</returns>
    object? this[ITypeKey key] { get; set; }

    /// <summary>
    /// Determines whether this type specification represents a Pascal string.
    /// </summary>
    /// <returns><see langword="true"/> when this type represents a Pascal string.</returns>
    bool IsPascalString();

    /// <summary>
    /// Gets the base type of this type specification.
    /// </summary>
    /// <remarks>
    /// Subranges return the type stored under the subrange base-type key. Other type forms return
    /// the current type specification. A malformed subrange without base-type metadata returns
    /// <see langword="null"/>.
    /// </remarks>
    ITypeSpec? BaseType { get; }
}
