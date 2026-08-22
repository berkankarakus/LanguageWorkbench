using Intermediate.Code.Interfaces;

namespace Intermediate.Code.Implementations;

/// <summary>
/// Identifies an attribute stored on an intermediate-code node.
/// </summary>
/// <param name="Name">The stable name of the attribute.</param>
public sealed record IntermediateCodeKey(string Name) : IIntermediateCodeKey
{
    /// <summary>
    /// Identifies the source line number of a node.
    /// </summary>
    public static IntermediateCodeKey Line { get; } = new(nameof(Line));

    /// <summary>
    /// Identifies a symbol-table entry associated with a node.
    /// </summary>
    public static IntermediateCodeKey Id { get; } = new(nameof(Id));

    /// <summary>
    /// Identifies the literal value associated with a node.
    /// </summary>
    public static IntermediateCodeKey Value { get; } = new(nameof(Value));

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
