using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;

namespace Intermediate.Factory;

/// <summary>
/// A factory for creating objects that implement the intermediate code.
/// </summary>
public static class IntermediateCodeFactory
{
    /// <summary>
    /// Creates an intermediate-code container.
    /// </summary>
    /// <returns>The intermediate code implementation.</returns>
    public static IIntermediateCode Create()
    {
        return new IntermediateCode();
    }

    /// <summary>
    /// Creates an intermediate-code node of the specified type.
    /// </summary>
    /// <param name="nodeType">The type to assign to the new node.</param>
    /// <returns>The new intermediate-code node.</returns>
    public static IIntermediateCodeNode CreateIntermediateCodeNode(IIntermediateCodeNodeType nodeType)
    {
        return new IntermediateCodeNode(nodeType);
    }
}
