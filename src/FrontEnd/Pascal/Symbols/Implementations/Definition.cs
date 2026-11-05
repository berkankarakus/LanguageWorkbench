using FrontEnd.Pascal.Symbols.Interfaces;

namespace FrontEnd.Pascal.Symbols.Implementations;

/// <summary>
/// Identifies how a Pascal symbol is declared or used.
/// </summary>
/// <param name="Name">The stable name of the definition.</param>
public sealed record Definition(string Name) : IDefinition
{
    /// <summary>Identifies a named constant.</summary>
    public static Definition Constant { get; } = new(nameof(Constant));

    /// <summary>Identifies an enumeration member.</summary>
    public static Definition EnumerationConstant { get; } = new(nameof(EnumerationConstant));

    /// <summary>Identifies a named type.</summary>
    public static Definition Type { get; } = new(nameof(Type));

    /// <summary>Identifies a variable.</summary>
    public static Definition Variable { get; } = new(nameof(Variable));

    /// <summary>Identifies a record field.</summary>
    public static Definition Field { get; } = new(nameof(Field));

    /// <summary>Identifies a value parameter.</summary>
    public static Definition ValueParameter { get; } = new(nameof(ValueParameter));

    /// <summary>Identifies a <c>VAR</c> parameter.</summary>
    public static Definition VarParameter { get; } = new(nameof(VarParameter));

    /// <summary>Identifies a program parameter.</summary>
    public static Definition ProgramParameter { get; } = new(nameof(ProgramParameter));

    /// <summary>Identifies the main program.</summary>
    public static Definition Program { get; } = new(nameof(Program));

    /// <summary>Identifies a procedure.</summary>
    public static Definition Procedure { get; } = new(nameof(Procedure));

    /// <summary>Identifies a function.</summary>
    public static Definition Function { get; } = new(nameof(Function));

    /// <summary>Identifies a symbol whose definition is not yet known.</summary>
    public static Definition Undefined { get; } = new(nameof(Undefined));

    /// <inheritdoc />
    public string Text => ToString().ToLowerInvariant();

    /// <inheritdoc />
    public override string ToString()
    {
        return Name switch
        {
            "EnumerationConstant" => "Enumeration Constant",
            "Field" => "Record Field",
            "ValueParameter" => "Value Parameter",
            "VarParameter" => "VAR Parameter",
            "ProgramParameter" => "Program Parameter",
            _ => Name
        };
    }
}
