using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Implementations;
using Intermediate.Symbols.Interfaces;

namespace Backend.Interpreter;

/// <summary>
/// Executes intermediate-code expression nodes.
/// </summary>
/// <param name="parent">The executor that provides the interpreter state.</param>
public class ExpressionExecutor(Executor parent) : Executor(parent)
{
    private static readonly HashSet<IntermediateCodeNodeType> ArithmeticOperations =
    [
        IntermediateCodeNodeType.Add,
        IntermediateCodeNodeType.Subtract,
        IntermediateCodeNodeType.Multiply,
        IntermediateCodeNodeType.FloatDivide,
        IntermediateCodeNodeType.IntegerDivide,
        IntermediateCodeNodeType.Mod,
    ];

    /// <summary>
    /// Evaluates an expression node.
    /// </summary>
    /// <param name="node">The expression node to evaluate.</param>
    /// <returns>The evaluated value, or <see langword="null"/> when no value can be produced.</returns>
    public object? Execute(IIntermediateCodeNode? node)
    {
        ArgumentNullException.ThrowIfNull(ExecutionContext.Code);

        if (node is null)
            return null;

        return node.Type switch
        {
            var type when type.Equals(IntermediateCodeNodeType.Variable) => ExecuteVariable(node),
            var type when type.Equals(IntermediateCodeNodeType.IntegerConstant) => node[
                IntermediateCodeKey.Value
            ],
            var type when type.Equals(IntermediateCodeNodeType.RealConstant) => node[
                IntermediateCodeKey.Value
            ],
            var type when type.Equals(IntermediateCodeNodeType.StringConstant) => node[
                IntermediateCodeKey.Value
            ],
            var type when type.Equals(IntermediateCodeNodeType.BooleanConstant) => node[
                IntermediateCodeKey.Value
            ],
            var type when type.Equals(IntermediateCodeNodeType.Negate) => ExecuteNegation(node),
            var type when type.Equals(IntermediateCodeNodeType.Not) => ExecuteNot(node),
            _ => ExecuteBinaryOperator(node),
        };
    }

    private static object? ExecuteVariable(IIntermediateCodeNode node)
    {
        ISymbolTableEntry? entry = node[IntermediateCodeKey.Id] as ISymbolTableEntry;
        return entry?[SymbolTableKey.DataValue];
    }

    private object? ExecuteNegation(IIntermediateCodeNode node)
    {
        return ExecuteFirstChild(node) switch
        {
            int value => -value,
            double value => -value,
            _ => null,
        };
    }

    private object? ExecuteNot(IIntermediateCodeNode node)
    {
        return ExecuteFirstChild(node) is bool value ? !value : null;
    }

    private object? ExecuteFirstChild(IIntermediateCodeNode node)
    {
        return node.Children.Count == 0 ? null : Execute(node.Children[0]);
    }

    private object? ExecuteBinaryOperator(IIntermediateCodeNode node)
    {
        if (node.Type is not IntermediateCodeNodeType nodeType || node.Children.Count < 2)
            return null;

        object? operand1 = Execute(node.Children[0]);
        object? operand2 = Execute(node.Children[1]);

        if (ArithmeticOperations.Contains(nodeType))
            return ExecuteArithmeticOperation(node, nodeType, operand1, operand2);

        if (
            nodeType.Equals(IntermediateCodeNodeType.And)
            || nodeType.Equals(IntermediateCodeNodeType.Or)
        )
        {
            return ExecuteBooleanOperation(nodeType, operand1, operand2);
        }

        return ExecuteRelationalOperation(nodeType, operand1, operand2);
    }

    private object? ExecuteArithmeticOperation(
        IIntermediateCodeNode node,
        IntermediateCodeNodeType nodeType,
        object? operand1,
        object? operand2
    )
    {
        if (
            TryGetIntegerArithmeticOperands(
                nodeType,
                operand1,
                operand2,
                out int integer1,
                out int integer2
            )
        )
        {
            return ExecuteIntegerOperation(node, nodeType, integer1, integer2);
        }

        if (!TryGetRealOperands(operand1, operand2, out double real1, out double real2))
            return null;

        return ExecuteRealOperation(node, nodeType, real1, real2);
    }

    private object? ExecuteIntegerOperation(
        IIntermediateCodeNode node,
        IntermediateCodeNodeType nodeType,
        int operand1,
        int operand2
    )
    {
        return nodeType switch
        {
            var type when type.Equals(IntermediateCodeNodeType.Add) => operand1 + operand2,
            var type when type.Equals(IntermediateCodeNodeType.Subtract) => operand1 - operand2,
            var type when type.Equals(IntermediateCodeNodeType.Multiply) => operand1 * operand2,
            var type when type.Equals(IntermediateCodeNodeType.IntegerDivide) => DivideIntegers(
                node,
                operand1,
                operand2
            ),
            var type when type.Equals(IntermediateCodeNodeType.Mod) => Modulo(
                node,
                operand1,
                operand2
            ),
            _ => null,
        };
    }

    private object? ExecuteRealOperation(
        IIntermediateCodeNode node,
        IntermediateCodeNodeType nodeType,
        double operand1,
        double operand2
    )
    {
        return nodeType switch
        {
            var type when type.Equals(IntermediateCodeNodeType.Add) => operand1 + operand2,
            var type when type.Equals(IntermediateCodeNodeType.Subtract) => operand1 - operand2,
            var type when type.Equals(IntermediateCodeNodeType.Multiply) => operand1 * operand2,
            var type when type.Equals(IntermediateCodeNodeType.FloatDivide) => DivideReals(
                node,
                operand1,
                operand2
            ),
            _ => null,
        };
    }

    private int DivideIntegers(IIntermediateCodeNode node, int dividend, int divisor)
    {
        if (divisor != 0)
            return dividend / divisor;

        RuntimeErrors.Flag(node, RuntimeErrorCode.DivisionByZero, this);
        return 0;
    }

    private int Modulo(IIntermediateCodeNode node, int dividend, int divisor)
    {
        if (divisor != 0)
            return dividend % divisor;

        RuntimeErrors.Flag(node, RuntimeErrorCode.DivisionByZero, this);
        return 0;
    }

    private double DivideReals(IIntermediateCodeNode node, double dividend, double divisor)
    {
        if (divisor != 0)
            return dividend / divisor;

        RuntimeErrors.Flag(node, RuntimeErrorCode.DivisionByZero, this);
        return 0.0;
    }

    private static object? ExecuteBooleanOperation(
        IntermediateCodeNodeType nodeType,
        object? operand1,
        object? operand2
    )
    {
        if (operand1 is not bool value1 || operand2 is not bool value2)
            return null;

        return nodeType.Equals(IntermediateCodeNodeType.And) ? value1 && value2 : value1 || value2;
    }

    private static bool? ExecuteRelationalOperation(
        IntermediateCodeNodeType nodeType,
        object? operand1,
        object? operand2
    )
    {
        if (operand1 is int integer1 && operand2 is int integer2)
            return Compare(nodeType, integer1.CompareTo(integer2));

        return TryGetRealOperands(operand1, operand2, out double real1, out double real2)
            ? Compare(nodeType, real1.CompareTo(real2))
            : null;
    }

    private static bool? Compare(IntermediateCodeNodeType nodeType, int comparison)
    {
        return nodeType switch
        {
            var type when type.Equals(IntermediateCodeNodeType.Equal) => comparison == 0,
            var type when type.Equals(IntermediateCodeNodeType.NotEqual) => comparison != 0,
            var type when type.Equals(IntermediateCodeNodeType.LessThan) => comparison < 0,
            var type when type.Equals(IntermediateCodeNodeType.LessThanOrEqual) => comparison <= 0,
            var type when type.Equals(IntermediateCodeNodeType.GreaterThan) => comparison > 0,
            var type when type.Equals(IntermediateCodeNodeType.GreaterThanOrEqual) => comparison
                >= 0,
            _ => null,
        };
    }

    private static bool TryGetRealOperands(
        object? operand1,
        object? operand2,
        out double value1,
        out double value2
    )
    {
        value1 = default;
        value2 = default;

        return TryGetRealValue(operand1, out value1) && TryGetRealValue(operand2, out value2);
    }

    private static bool TryGetIntegerArithmeticOperands(
        IntermediateCodeNodeType nodeType,
        object? operand1,
        object? operand2,
        out int value1,
        out int value2
    )
    {
        value1 = default;
        value2 = default;

        return !nodeType.Equals(IntermediateCodeNodeType.FloatDivide)
            && operand1 is int integer1
            && operand2 is int integer2
            && AssignIntegerOperands(integer1, integer2, out value1, out value2);
    }

    private static bool AssignIntegerOperands(
        int operand1,
        int operand2,
        out int value1,
        out int value2
    )
    {
        value1 = operand1;
        value2 = operand2;
        return true;
    }

    private static bool TryGetRealValue(object? operand, out double value)
    {
        switch (operand)
        {
            case int integer:
                value = integer;
                return true;
            case double real:
                value = real;
                return true;
            default:
                value = default;
                return false;
        }
    }
}
