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
    public IIntermediateCodeNodeType NodeType { get; } = nodeType;

    /// <inheritdoc />
    public IIntermediateCodeNode? Parent { get; set; }

    /// <inheritdoc />
    public List<IIntermediateCodeNode> Children { get; } = [];

    private readonly Dictionary<IIntermediateCodeKey, object> _attributes = new();

    /// <inheritdoc />
    public IIntermediateCodeNode? AddChild(IIntermediateCodeNode? child)
    {
        if (child is not null)
        {
            Children.Add(child);
            child.Parent = this;
        }

        return child;
    }

    /// <inheritdoc />
    public object? this[IIntermediateCodeKey key]
    {
        get => _attributes.GetValueOrDefault(key);
        set
        {
            if (value is null)
                _attributes.Remove(key);
            else
                _attributes[key] = value;
        }
    }

    /// <inheritdoc />
    public IIntermediateCodeNode Copy()
    {
        IIntermediateCodeNode copy = IntermediateCodeFactory.CreateIntermediateCodeNode(NodeType);

        foreach (var (key, value) in _attributes)
            copy[key] = value;

        return copy;
    }

    /// <inheritdoc />
    public override string? ToString()
    {
        return NodeType.ToString();
    }
}
