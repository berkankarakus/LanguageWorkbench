using Intermediate.Code.Interfaces;
using Intermediate.Factory;

namespace Intermediate.Code.Implementations;

/// <summary>
/// Represents a mutable node in an intermediate-code tree.
/// </summary>
/// <param name="nodeType">The type that describes the node's syntactic role.</param>
public class IntermediateCodeNode(IIntermediateCodeNodeType nodeType) : IIntermediateCodeNode
{
    /// <inheritdoc />
    public IIntermediateCodeNodeType Type { get; } = nodeType;

    /// <inheritdoc />
    public IIntermediateCodeNode? Parent { get; set; }

    /// <inheritdoc />
    public List<IIntermediateCodeNode> Children { get; } = [];

    /// <summary>
    /// Gets the attributes associated with this node.
    /// </summary>
    public Dictionary<IIntermediateCodeKey, object> Attributes { get; } = new();

    /// <inheritdoc />
    public IIntermediateCodeNode? AddChild(IIntermediateCodeNode? child)
    {
        if (child is null)
            return child;

        Children.Add(child);
        child.Parent = this;

        return child;
    }

    /// <inheritdoc />
    public object? this[IIntermediateCodeKey key]
    {
        get => Attributes.GetValueOrDefault(key);
        set
        {
            if (value is null)
                Attributes.Remove(key);
            else
                Attributes[key] = value;
        }
    }

    /// <inheritdoc />
    public IIntermediateCodeNode Copy()
    {
        IIntermediateCodeNode copy = IntermediateCodeFactory.CreateIntermediateCodeNode(Type);

        foreach (var (key, value) in Attributes)
            copy[key] = value;

        return copy;
    }

    /// <inheritdoc />
    public override string? ToString()
    {
        return Type.ToString();
    }
}
