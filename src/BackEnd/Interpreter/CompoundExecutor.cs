using Intermediate.Code.Interfaces;

namespace Backend.Interpreter;

public class CompoundExecutor(Executor parent) : Executor(parent)
{
    public object? Execute(IIntermediateCodeNode node)
    {
        // Loop over the children of the COMPOUND node and execute each child.
        StatementExecutor statementExecutor = new(this);
        List<IIntermediateCodeNode> children = node.Children;

        foreach (var child in children)
            statementExecutor.Execute(child);

        return null;
    }
}
