namespace Intermediate.Code.Interfaces;

/// <summary>
/// Defines a node in the intermediate-code tree.
/// </summary>
public interface IIntermediateCodeNode
{
    /// <summary>
    /// Gets the type that identifies this node's syntactic role.
    /// </summary>
    /// <returns>The node type.</returns>
    IIntermediateCodeNodeType Type { get; }

    /// <summary>
    /// Gets the parent of this node.
    /// </summary>
    /// <returns>The parent node.</returns>
    IIntermediateCodeNode? Parent { get; set; }

    /// <summary>
    /// Adds a child node to this node.
    /// </summary>
    /// <param name="child">The child node to add.</param>
    /// <returns>The added child node.</returns>
    IIntermediateCodeNode? AddChild(IIntermediateCodeNode? child);

    /// <summary>
    /// Gets this node's children in source order.
    /// </summary>
    /// <returns>The child nodes.</returns>
    List<IIntermediateCodeNode> Children { get; }

    /// <summary>
    /// Gets or sets an attribute value associated with this node.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    /// <returns>The attribute value.</returns>
    object? this[IIntermediateCodeKey key] { get; set; }

    /// <summary>
    /// Creates a copy of this node without copying its children.
    /// </summary>
    /// <returns>A copy of this node.</returns>
    IIntermediateCodeNode Copy();
}
