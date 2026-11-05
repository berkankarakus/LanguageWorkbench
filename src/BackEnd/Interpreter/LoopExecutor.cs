using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;

namespace Backend.Interpreter;

/// <summary>
/// Executes an intermediate-code loop node.
/// </summary>
/// <param name="parent">The executor that provides the interpreter state.</param>
public sealed class LoopExecutor(Executor parent) : Executor(parent)
{
    /// <summary>
    /// Executes a loop until its <c>TEST</c> child evaluates to <see langword="true"/>.
    /// </summary>
    /// <param name="node">The root <c>LOOP</c> node.</param>
    /// <returns>Always <see langword="null"/>.</returns>
    public object? Execute(IIntermediateCodeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        bool exitLoop = false;
        List<IIntermediateCodeNode> loopChildren = node.Children;
        IIntermediateCodeNode expressionNode = GetTestExpression(loopChildren);

        ExpressionExecutor expressionExecutor = new(this);
        StatementExecutor statementExecutor = new(this);

        // Loop until the TEST expression value is true.
        while (!exitLoop)
        {
            // Count the loop statement itself.
            IncrementExecutionCount();

            // Execute the children of the LOOP node.
            foreach (var child in loopChildren)
            {
                if (IntermediateCodeNodeType.Test.Equals(child.Type))
                    exitLoop = expressionExecutor.Execute(expressionNode) is true;

                // Statement node.
                else
                    statementExecutor.Execute(child);

                if (exitLoop)
                    break;
            }
        }

        return null;
    }

    private static IIntermediateCodeNode GetTestExpression(
        IEnumerable<IIntermediateCodeNode> loopChildren
    )
    {
        IIntermediateCodeNode? testNode = null;

        foreach (IIntermediateCodeNode child in loopChildren)
        {
            if (!IntermediateCodeNodeType.Test.Equals(child.Type))
                continue;

            if (testNode is not null)
                throw new InvalidOperationException("A LOOP node must have exactly one TEST child.");

            testNode = child;
        }

        return
            testNode?.Children.FirstOrDefault()
            ?? throw new InvalidOperationException("A LOOP node must have a TEST child with an expression.");
    }
}
