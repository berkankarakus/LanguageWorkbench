using Intermediate.Code.Interfaces;

namespace Backend.Interpreter;

/// <summary>
/// Executes an intermediate-code <c>IF</c> node.
/// </summary>
/// <param name="parent">The executor that provides the interpreter state.</param>
public sealed class IfExecutor(Executor parent) : Executor(parent)
{
    /// <summary>
    /// Evaluates the condition and executes the selected branch, if present.
    /// </summary>
    /// <param name="node">The <c>IF</c> node to execute.</param>
    /// <returns>Always <see langword="null"/>.</returns>
    public object? Execute(IIntermediateCodeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node.Children.Count < 2)
            throw new InvalidOperationException("An IF node must have a condition and a THEN branch.");

        IIntermediateCodeNode conditionNode = node.Children[0];
        IIntermediateCodeNode? statementNode = IsConditionTrue(conditionNode)
            ? node.Children[1]
            : node.Children.Count > 2
                ? node.Children[2]
                : null;

        StatementExecutor statementExecutor = new(this);
        _ = statementExecutor.Execute(statementNode);

        IncrementExecutionCount();
        return null;
    }

    private bool IsConditionTrue(IIntermediateCodeNode conditionNode)
    {
        ExpressionExecutor expressionExecutor = new(this);
        return expressionExecutor.Execute(conditionNode) is true;
    }
}
