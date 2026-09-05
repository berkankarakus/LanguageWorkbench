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
        ArgumentNullException.ThrowIfNull(Code);

        if (node is null)
            return null;

        // Send a message about the current source line.
        SendSourceLineMessage(node);

        if (IntermediateCodeNodeType.Compound.Equals(node.Type))
        {
            CompoundExecutor compoundExecutor = new(this);
            return compoundExecutor.Execute(node);
        }

        if (IntermediateCodeNodeType.Assignment.Equals(node.Type))
        {
            AssignmentExecutor assignmentExecutor = new(this);
            return assignmentExecutor.Execute(node);
        }

        if (IntermediateCodeNodeType.NoOp.Equals(node.Type))
            return null;

        RuntimeErrorHandler.Flag(node, RuntimeErrorCode.UnimplementedFeature, this);
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
