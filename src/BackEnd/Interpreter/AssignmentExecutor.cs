using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Implementations;
using Intermediate.Symbols.Interfaces;
using Messaging;

namespace Backend.Interpreter;

/// <summary>
/// Executes an intermediate-code assignment node.
/// </summary>
/// <param name="parent">The executor that provides the interpreter state.</param>
public class AssignmentExecutor(Executor parent) : Executor(parent)
{
    /// <summary>
    /// Evaluates the assignment expression and stores its value in the target variable.
    /// </summary>
    /// <param name="node">The assignment node to execute.</param>
    /// <returns>The assigned value.</returns>
    public object? Execute(IIntermediateCodeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        // The ASSIGN node's children are the target variable and the expression.
        if (node.Children.Count < 2)
        {
            throw new InvalidOperationException(
                "An assignment node must have a target and an expression."
            );
        }

        IIntermediateCodeNode variableNode = node.Children[0];
        IIntermediateCodeNode expressionNode = node.Children[1];

        ISymbolTableEntry variableId =
            variableNode[IntermediateCodeKey.Id] as ISymbolTableEntry
            ?? throw new InvalidOperationException(
                "An assignment target must reference a symbol table entry."
            );

        // Execute the expression and get its value.
        ExpressionExecutor expressionExecutor = new(this);
        object? value = expressionExecutor.Execute(expressionNode);

        // Set the value as an attribute of the variable's symbol table entry.
        variableId[SymbolTableKey.DataValue] = value;

        SendMessage(node, variableId.Name, value);

        IncrementExecutionCount();
        return value;
    }

    private void SendMessage(IIntermediateCodeNode node, string name, object? value)
    {
        // Send an ASSIGN message.
        if (node[IntermediateCodeKey.Line] is int lineNumber)
        {
            SendMessage(
                new Message(MessageType.Assign, new object?[] { lineNumber, name, value })
            );
        }
    }
}
