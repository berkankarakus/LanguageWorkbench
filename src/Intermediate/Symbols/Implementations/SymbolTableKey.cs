using Intermediate.Symbols.Interfaces;

namespace Intermediate.Symbols.Implementations;

/// <summary>
/// Defines language-independent attribute keys used by symbol table entries.
/// </summary>
public sealed class SymbolTableKey : ISymbolTableKey
{
    private SymbolTableKey(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the display name of the key.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the key for a constant's value.
    /// </summary>
    public static SymbolTableKey ConstantValue { get; } = new(nameof(ConstantValue));

    /// <summary>
    /// Gets the key for a variable's current runtime value.
    /// </summary>
    public static SymbolTableKey DataValue { get; } = new(nameof(DataValue));

    /// <summary>
    /// Gets the key for a routine's executable code.
    /// </summary>
    public static SymbolTableKey RoutineCode { get; } = new(nameof(RoutineCode));

    /// <summary>
    /// Gets the key for a routine's local symbol table.
    /// </summary>
    public static SymbolTableKey RoutineSymbolTable { get; } = new(nameof(RoutineSymbolTable));

    /// <summary>
    /// Gets the key for a routine's intermediate code.
    /// </summary>
    public static SymbolTableKey RoutineIntermediateCode { get; } = new(nameof(RoutineIntermediateCode));

    /// <summary>
    /// Gets the key for a routine's formal parameters.
    /// </summary>
    public static SymbolTableKey RoutineParams { get; } = new(nameof(RoutineParams));

    /// <summary>
    /// Gets the key for a routine's nested routines.
    /// </summary>
    public static SymbolTableKey RoutineRoutines { get; } = new(nameof(RoutineRoutines));

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}
