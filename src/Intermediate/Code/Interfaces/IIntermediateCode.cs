namespace Intermediate.Code.Interfaces;

/// <summary>
/// Holds the intermediate representation generated during parsing.
/// </summary>
public interface IIntermediateCode
{
    /// <summary>
    /// Gets or sets the root node of the intermediate-code tree.
    /// </summary>
    IIntermediateCodeNode? Root { get; set; }
}
