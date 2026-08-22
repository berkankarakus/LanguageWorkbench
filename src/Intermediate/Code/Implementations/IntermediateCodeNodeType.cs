using Intermediate.Code.Interfaces;

namespace Intermediate.Code.Implementations;

/// <summary>
/// Defines the language-independent node types used in the intermediate-code tree.
/// </summary>
/// <param name="Name">The stable name of the node type.</param>
public sealed record IntermediateCodeNodeType(string Name) : IIntermediateCodeNodeType
{
    // Program structure
    public static IntermediateCodeNodeType Program { get; } = new(nameof(Program));
    public static IntermediateCodeNodeType Procedure { get; } = new(nameof(Procedure));
    public static IntermediateCodeNodeType Function { get; } = new(nameof(Function));

    // Statements
    public static IntermediateCodeNodeType Compound { get; } = new(nameof(Compound));
    public static IntermediateCodeNodeType Assignment { get; } = new(nameof(Assignment));
    public static IntermediateCodeNodeType Loop { get; } = new(nameof(Loop));
    public static IntermediateCodeNodeType Test { get; } = new(nameof(Test));
    public static IntermediateCodeNodeType Call { get; } = new(nameof(Call));
    public static IntermediateCodeNodeType Parameters { get; } = new(nameof(Parameters));
    public static IntermediateCodeNodeType If { get; } = new(nameof(If));
    public static IntermediateCodeNodeType Select { get; } = new(nameof(Select));
    public static IntermediateCodeNodeType SelectBranch { get; } = new(nameof(SelectBranch));
    public static IntermediateCodeNodeType SelectConstant { get; } = new(nameof(SelectConstant));
    public static IntermediateCodeNodeType NoOp { get; } = new(nameof(NoOp));

    // Relational operators
    public static IntermediateCodeNodeType Equal { get; } = new(nameof(Equal));
    public static IntermediateCodeNodeType NotEqual { get; } = new(nameof(NotEqual));
    public static IntermediateCodeNodeType LessThan { get; } = new(nameof(LessThan));
    public static IntermediateCodeNodeType LessThanOrEqual { get; } = new(nameof(LessThanOrEqual));
    public static IntermediateCodeNodeType GreaterThan { get; } = new(nameof(GreaterThan));
    public static IntermediateCodeNodeType GreaterThanOrEqual { get; } = new(nameof(GreaterThanOrEqual));
    public static IntermediateCodeNodeType Not { get; } = new(nameof(Not));

    // Additive operators
    public static IntermediateCodeNodeType Add { get; } = new(nameof(Add));
    public static IntermediateCodeNodeType Subtract { get; } = new(nameof(Subtract));
    public static IntermediateCodeNodeType Or { get; } = new(nameof(Or));
    public static IntermediateCodeNodeType Negate { get; } = new(nameof(Negate));

    // Multiplicative operators
    public static IntermediateCodeNodeType Multiply { get; } = new(nameof(Multiply));
    public static IntermediateCodeNodeType IntegerDivide { get; } = new(nameof(IntegerDivide));
    public static IntermediateCodeNodeType FloatDivide { get; } = new(nameof(FloatDivide));
    public static IntermediateCodeNodeType Mod { get; } = new(nameof(Mod));
    public static IntermediateCodeNodeType And { get; } = new(nameof(And));

    // Operands
    public static IntermediateCodeNodeType Variable { get; } = new(nameof(Variable));
    public static IntermediateCodeNodeType Subscripts { get; } = new(nameof(Subscripts));
    public static IntermediateCodeNodeType Field { get; } = new(nameof(Field));
    public static IntermediateCodeNodeType IntegerConstant { get; } = new(nameof(IntegerConstant));
    public static IntermediateCodeNodeType RealConstant { get; } = new(nameof(RealConstant));
    public static IntermediateCodeNodeType StringConstant { get; } = new(nameof(StringConstant));
    public static IntermediateCodeNodeType BooleanConstant { get; } = new(nameof(BooleanConstant));

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
