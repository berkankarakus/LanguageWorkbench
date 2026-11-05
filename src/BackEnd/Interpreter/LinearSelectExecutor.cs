using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;

namespace Backend.Interpreter;

/// <summary>
/// Executes an intermediate-code <c>SELECT</c> node by linearly scanning its branches.
/// </summary>
/// <param name="parent">The executor that provides the interpreter state.</param>
public sealed class LinearSelectExecutor(Executor parent) : Executor(parent)
{
    /// <summary>
    /// Evaluates the CASE expression and executes the first matching branch.
    /// </summary>
    /// <param name="node">The <c>SELECT</c> node to execute.</param>
    /// <returns>Always <see langword="null"/>.</returns>
    public object? Execute(IIntermediateCodeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node.Children.Count == 0)
            throw new InvalidOperationException("A SELECT node must have a CASE expression.");

        object? selectedValue = new ExpressionExecutor(this).Execute(node.Children[0]);
        IIntermediateCodeNode? statementNode = FindMatchingStatement(node.Children, selectedValue);

        if (statementNode is not null)
        {
            StatementExecutor statementExecutor = new(this);
            _ = statementExecutor.Execute(statementNode);
        }

        IncrementExecutionCount();
        return null;
    }

    /// <summary>
    /// Finds the statement belonging to the first CASE branch that matches the selected value.
    /// </summary>
    /// <param name="selectChildren">The CASE expression followed by the branch nodes.</param>
    /// <param name="selectedValue">The evaluated CASE expression value.</param>
    /// <returns>The selected branch's statement, or <see langword="null"/> when no branch matches.</returns>
    private static IIntermediateCodeNode? FindMatchingStatement(
        List<IIntermediateCodeNode> selectChildren,
        object? selectedValue
    )
    {
        for (int index = 1; index < selectChildren.Count; ++index)
        {
            IIntermediateCodeNode branchNode = selectChildren[index];

            if (MatchesBranch(branchNode, selectedValue))
                return GetBranchStatement(branchNode);
        }

        return null;
    }

    /// <summary>
    /// Determines whether a CASE branch has a constant equal to the selected value.
    /// </summary>
    /// <param name="branchNode">The <c>SELECT_BRANCH</c> node to inspect.</param>
    /// <param name="selectedValue">The evaluated CASE expression value.</param>
    /// <returns><see langword="true"/> when a branch constant matches.</returns>
    private static bool MatchesBranch(IIntermediateCodeNode branchNode, object? selectedValue)
    {
        IIntermediateCodeNode constantsNode = GetConstantsNode(branchNode);

        foreach (IIntermediateCodeNode constantNode in constantsNode.Children)
        {
            object? constantValue = constantNode[IntermediateCodeKey.Value];

            if (Equals(selectedValue, constantValue))
                return true;
        }

        return false;
    }

    private static IIntermediateCodeNode GetConstantsNode(IIntermediateCodeNode branchNode)
    {
        if (!IntermediateCodeNodeType.SelectBranch.Equals(branchNode.Type))
            throw new InvalidOperationException("A SELECT node can contain only SELECT_BRANCH nodes.");

        if (branchNode.Children.Count == 0)
            throw new InvalidOperationException("A SELECT_BRANCH node must have a SELECT_CONSTANTS child.");

        IIntermediateCodeNode constantsNode = branchNode.Children[0];

        if (!IntermediateCodeNodeType.SelectConstants.Equals(constantsNode.Type))
            throw new InvalidOperationException("A SELECT_BRANCH node must begin with a SELECT_CONSTANTS node.");

        return constantsNode;
    }

    private static IIntermediateCodeNode GetBranchStatement(IIntermediateCodeNode branchNode)
    {
        if (branchNode.Children.Count < 2)
            throw new InvalidOperationException("A SELECT_BRANCH node must have a statement child.");

        return branchNode.Children[1];
    }
}
