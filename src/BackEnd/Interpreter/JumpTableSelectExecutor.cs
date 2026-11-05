using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;

namespace Backend.Interpreter;

/// <summary>
/// Executes <c>SELECT</c> nodes through a cached jump table.
/// </summary>
/// <param name="parent">The executor that provides the interpreter state.</param>
public sealed class JumpTableSelectExecutor(Executor parent) : Executor(parent)
{
    /// <summary>
    /// Evaluates a CASE expression and executes the branch found through its jump table.
    /// </summary>
    /// <param name="node">The <c>SELECT</c> node to execute.</param>
    /// <returns>Always <see langword="null"/>.</returns>
    public object? Execute(IIntermediateCodeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node.Children.Count == 0)
            throw new InvalidOperationException("A SELECT node must have a CASE expression.");

        Dictionary<object, IIntermediateCodeNode> jumpTable = GetOrCreateJumpTable(node);
        IIntermediateCodeNode expressionNode = node.Children[0];

        object? selectedValue = new ExpressionExecutor(this).Execute(expressionNode);

        if (TryGetSelectedStatement(jumpTable, selectedValue, out IIntermediateCodeNode? statementNode))
            _ = new StatementExecutor(this).Execute(statementNode);

        IncrementExecutionCount();
        return null;
    }

    /// <summary>
    /// Gets the jump table cached for a SELECT node, creating it on first use.
    /// </summary>
    /// <param name="node">The <c>SELECT</c> node that owns the jump table.</param>
    /// <returns>The cached jump table.</returns>
    private Dictionary<object, IIntermediateCodeNode> GetOrCreateJumpTable(
        IIntermediateCodeNode node
    )
    {
        return ExecutionContext.GetOrCreateJumpTable(node, CreateJumpTable);
    }

    /// <summary>
    /// Creates a mapping from each CASE constant to its branch statement.
    /// </summary>
    /// <param name="node">The <c>SELECT</c> node to map.</param>
    /// <returns>The newly created jump table.</returns>
    private static Dictionary<object, IIntermediateCodeNode> CreateJumpTable(IIntermediateCodeNode node)
    {
        Dictionary<object, IIntermediateCodeNode> jumpTable = [];

        // Loop over children that are SELECT_BRANCH nodes.
        var selectChildren = node.Children;

        for (int index = 1; index < selectChildren.Count; ++index)
        {
            IIntermediateCodeNode branchNode = selectChildren[index];
            IIntermediateCodeNode constantsNode = GetConstantsNode(branchNode);
            IIntermediateCodeNode statementNode = GetBranchStatement(branchNode);

            // Loop over the constants children of the branch's CONSTANTS_NODE.
            var constantsList = constantsNode.Children;

            foreach (IIntermediateCodeNode constantNode in constantsList)
            {
                object value = constantNode[IntermediateCodeKey.Value]
                    ?? throw new InvalidOperationException("A SELECT_CONSTANTS child must have a value.");

                if (!jumpTable.TryAdd(value, statementNode))
                    throw new InvalidOperationException("A SELECT node cannot reuse a CASE constant.");
            }
        }

        return jumpTable;
    }

    private static bool TryGetSelectedStatement(
        Dictionary<object, IIntermediateCodeNode> jumpTable,
        object? selectedValue,
        out IIntermediateCodeNode? statementNode
    )
    {
        if (selectedValue is null)
        {
            statementNode = null;
            return false;
        }

        return jumpTable.TryGetValue(selectedValue, out statementNode);
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
