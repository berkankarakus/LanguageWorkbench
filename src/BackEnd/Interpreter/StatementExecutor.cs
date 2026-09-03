using Intermediate.Code.Interfaces;
using Intermediate.Code.Implementations;

namespace Backend.Interpreter;

/// <summary>
/// Executes an intermediate-code statement node.
/// </summary>
/// <param name="parent">The executor that provides the interpreter state.</param>
public sealed class StatementExecutor(Executor parent) : Executor(parent)
{
    /// <summary>
    /// Executes the supplied statement node.
    /// </summary>
    /// <param name="node">The statement node to execute.</param>
    public void Execute(IIntermediateCodeNode? node)
    {
        ArgumentNullException.ThrowIfNull(Code);

        if (!IntermediateCodeNodeType.Compound.Equals(node?.NodeType))
        {
            IncrementExecutionCount();
            return;
        }

        foreach (IIntermediateCodeNode child in node.Children)
            Execute(child);
    }
}
