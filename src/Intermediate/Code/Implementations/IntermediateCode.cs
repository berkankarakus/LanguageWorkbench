using Intermediate.Code.Interfaces;

namespace Intermediate.Code.Implementations;

/// <summary>
/// Represents the intermediate code generated during parsing.
/// </summary>
public sealed class IntermediateCode : IIntermediateCode
{
    /// <inheritdoc />
    public IIntermediateCodeNode? Root { get; set; }
}
