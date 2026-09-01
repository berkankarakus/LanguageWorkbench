using Intermediate.Code.Interfaces;

namespace Intermediate.Code.Implementations;

/// <summary>
/// Defines the language-independent node types used in the intermediate-code tree.
/// </summary>
/// <param name="Name">The stable name of the node type.</param>
public sealed record IntermediateCodeNodeType(string Name) : IIntermediateCodeNodeType
{
    // Program structure
    /// <summary>Represents a program.</summary>
    public static IntermediateCodeNodeType Program { get; } = new(nameof(Program));
    /// <summary>Represents a procedure declaration or call.</summary>
    public static IntermediateCodeNodeType Procedure { get; } = new(nameof(Procedure));
    /// <summary>Represents a function declaration or call.</summary>
    public static IntermediateCodeNodeType Function { get; } = new(nameof(Function));

    // Statements
    /// <summary>Represents a compound statement.</summary>
    public static IntermediateCodeNodeType Compound { get; } = new(nameof(Compound));
    /// <summary>Represents an assignment statement.</summary>
    public static IntermediateCodeNodeType Assignment { get; } = new(nameof(Assignment));
    /// <summary>Represents a loop statement.</summary>
    public static IntermediateCodeNodeType Loop { get; } = new(nameof(Loop));
    /// <summary>Represents a conditional test.</summary>
    public static IntermediateCodeNodeType Test { get; } = new(nameof(Test));
    /// <summary>Represents a routine call.</summary>
    public static IntermediateCodeNodeType Call { get; } = new(nameof(Call));
    /// <summary>Represents a list of routine parameters.</summary>
    public static IntermediateCodeNodeType Parameters { get; } = new(nameof(Parameters));
    /// <summary>Represents an if statement.</summary>
    public static IntermediateCodeNodeType If { get; } = new(nameof(If));
    /// <summary>Represents a selection statement.</summary>
    public static IntermediateCodeNodeType Select { get; } = new(nameof(Select));
    /// <summary>Represents a branch of a selection statement.</summary>
    public static IntermediateCodeNodeType SelectBranch { get; } = new(nameof(SelectBranch));
    /// <summary>Represents a selection constant.</summary>
    public static IntermediateCodeNodeType SelectConstant { get; } = new(nameof(SelectConstant));
    /// <summary>Represents a statement with no operation.</summary>
    public static IntermediateCodeNodeType NoOp { get; } = new(nameof(NoOp));

    // Relational operators
    /// <summary>Represents equality comparison.</summary>
    public static IntermediateCodeNodeType Equal { get; } = new(nameof(Equal));
    /// <summary>Represents inequality comparison.</summary>
    public static IntermediateCodeNodeType NotEqual { get; } = new(nameof(NotEqual));
    /// <summary>Represents a less-than comparison.</summary>
    public static IntermediateCodeNodeType LessThan { get; } = new(nameof(LessThan));
    /// <summary>Represents a less-than-or-equal comparison.</summary>
    public static IntermediateCodeNodeType LessThanOrEqual { get; } = new(nameof(LessThanOrEqual));
    /// <summary>Represents a greater-than comparison.</summary>
    public static IntermediateCodeNodeType GreaterThan { get; } = new(nameof(GreaterThan));
    /// <summary>Represents a greater-than-or-equal comparison.</summary>
    public static IntermediateCodeNodeType GreaterThanOrEqual { get; } = new(nameof(GreaterThanOrEqual));
    /// <summary>Represents logical negation.</summary>
    public static IntermediateCodeNodeType Not { get; } = new(nameof(Not));

    // Additive operators
    /// <summary>Represents addition.</summary>
    public static IntermediateCodeNodeType Add { get; } = new(nameof(Add));
    /// <summary>Represents subtraction.</summary>
    public static IntermediateCodeNodeType Subtract { get; } = new(nameof(Subtract));
    /// <summary>Represents logical disjunction.</summary>
    public static IntermediateCodeNodeType Or { get; } = new(nameof(Or));
    /// <summary>Represents arithmetic negation.</summary>
    public static IntermediateCodeNodeType Negate { get; } = new(nameof(Negate));

    // Multiplicative operators
    /// <summary>Represents multiplication.</summary>
    public static IntermediateCodeNodeType Multiply { get; } = new(nameof(Multiply));
    /// <summary>Represents integer division.</summary>
    public static IntermediateCodeNodeType IntegerDivide { get; } = new(nameof(IntegerDivide));
    /// <summary>Represents floating-point division.</summary>
    public static IntermediateCodeNodeType FloatDivide { get; } = new(nameof(FloatDivide));
    /// <summary>Represents modulo division.</summary>
    public static IntermediateCodeNodeType Mod { get; } = new(nameof(Mod));
    /// <summary>Represents logical conjunction.</summary>
    public static IntermediateCodeNodeType And { get; } = new(nameof(And));

    // Operands
    /// <summary>Represents a variable reference.</summary>
    public static IntermediateCodeNodeType Variable { get; } = new(nameof(Variable));
    /// <summary>Represents array subscripts.</summary>
    public static IntermediateCodeNodeType Subscripts { get; } = new(nameof(Subscripts));
    /// <summary>Represents a record field selection.</summary>
    public static IntermediateCodeNodeType Field { get; } = new(nameof(Field));
    /// <summary>Represents an integer literal.</summary>
    public static IntermediateCodeNodeType IntegerConstant { get; } = new(nameof(IntegerConstant));
    /// <summary>Represents a real literal.</summary>
    public static IntermediateCodeNodeType RealConstant { get; } = new(nameof(RealConstant));
    /// <summary>Represents a string literal.</summary>
    public static IntermediateCodeNodeType StringConstant { get; } = new(nameof(StringConstant));
    /// <summary>Represents a Boolean literal.</summary>
    public static IntermediateCodeNodeType BooleanConstant { get; } = new(nameof(BooleanConstant));

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
