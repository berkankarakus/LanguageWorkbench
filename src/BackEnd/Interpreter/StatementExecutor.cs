using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Messaging;

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
    public object? Execute(IIntermediateCodeNode? node)
    {
        ArgumentNullException.ThrowIfNull(ExecutionContext.Code);

        if (node is null)
            return null;

        var nodeType = node.Type;

        // Send a message about the current source line.
        SendSourceLineMessage(node);

        if (IntermediateCodeNodeType.Compound.Equals(nodeType))
        {
            CompoundExecutor compoundExecutor = new(this);
            return compoundExecutor.Execute(node);
        }

        if (IntermediateCodeNodeType.Assignment.Equals(nodeType))
        {
            AssignmentExecutor assignmentExecutor = new(this);
            return assignmentExecutor.Execute(node);
        }

        if (IntermediateCodeNodeType.Loop.Equals(nodeType))
        {
            LoopExecutor loopExecutor = new(this);
            return loopExecutor.Execute(node);
        }

        if (IntermediateCodeNodeType.If.Equals(nodeType))
        {
            IfExecutor ifExecutor = new(this);
            return ifExecutor.Execute(node);
        }

        if (IntermediateCodeNodeType.Select.Equals(nodeType))
        {
            JumpTableSelectExecutor selectExecutor = new(this);
            return selectExecutor.Execute(node);
        }

        if (IntermediateCodeNodeType.NoOp.Equals(nodeType))
            return null;

        RuntimeErrors.Flag(node, RuntimeErrorCode.UnimplementedFeature, this);
        return null;
    }

    /// <summary>
    /// Sends a message about the current source line when one is available.
    /// </summary>
    /// <param name="node">The node associated with the source line.</param>
    private void SendSourceLineMessage(IIntermediateCodeNode? node)
    {
        // Send the SourceLine message.
        if (node?[IntermediateCodeKey.Line] is int lineNumber)
            SendMessage(new Message(MessageType.SourceLine, lineNumber));
    }
}
