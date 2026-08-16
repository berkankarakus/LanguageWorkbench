using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Represents a Pascal-specific token type.
/// </summary>
public sealed class PascalTokenType : ITokenType
{
    private PascalTokenType(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the token type used when scanning encounters an error.
    /// </summary>
    public static PascalTokenType Error { get; } = new("ERROR");

    /// <summary>
    /// Gets the token type for string literals.
    /// </summary>
    public static PascalTokenType String { get; } = new("STRING");

    private string Name { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
