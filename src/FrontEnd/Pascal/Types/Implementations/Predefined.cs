using FrontEnd.Pascal.Symbols.Implementations;
using FrontEnd.Pascal.Types.Factory;
using FrontEnd.Pascal.Types.Interfaces;
using Intermediate.Symbols.Implementations;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Types.Implementations;

/// <summary>
/// Defines Pascal's predefined type specifications and their symbol-table entries.
/// </summary>
public sealed class Predefined
{
    /// <summary>Gets the predefined <c>integer</c> type.</summary>
    public ITypeSpec IntegerType { get; private set; } = null!;

    /// <summary>Gets the predefined <c>real</c> type.</summary>
    public ITypeSpec RealType { get; private set; } = null!;

    /// <summary>Gets the predefined <c>boolean</c> type.</summary>
    public ITypeSpec BooleanType { get; private set; } = null!;

    /// <summary>Gets the predefined <c>char</c> type.</summary>
    public ITypeSpec CharType { get; private set; } = null!;

    /// <summary>Gets the placeholder type used after an invalid type reference.</summary>
    public ITypeSpec UndefinedType { get; private set; } = null!;

    /// <summary>Gets the symbol-table entry for <c>integer</c>.</summary>
    public ISymbolTableEntry IntegerId { get; private set; } = null!;

    /// <summary>Gets the symbol-table entry for <c>real</c>.</summary>
    public ISymbolTableEntry RealId { get; private set; } = null!;

    /// <summary>Gets the symbol-table entry for <c>boolean</c>.</summary>
    public ISymbolTableEntry BooleanId { get; private set; } = null!;

    /// <summary>Gets the symbol-table entry for <c>char</c>.</summary>
    public ISymbolTableEntry CharId { get; private set; } = null!;

    /// <summary>Gets the symbol-table entry for the <c>false</c> Boolean constant.</summary>
    public ISymbolTableEntry FalseId { get; private set; } = null!;

    /// <summary>Gets the symbol-table entry for the <c>true</c> Boolean constant.</summary>
    public ISymbolTableEntry TrueId { get; private set; } = null!;

    /// <summary>
    /// Initializes Pascal's predefined types and constants in the outermost symbol table.
    /// </summary>
    /// <param name="symbolTableStack">The symbol-table stack that owns the outermost scope.</param>
    public void Initialize(ISymbolTableStack symbolTableStack)
    {
        ArgumentNullException.ThrowIfNull(symbolTableStack);

        InitializeTypes(symbolTableStack);
        InitializeConstants(symbolTableStack);
    }

    /// <summary>
    /// Initializes the predefined type specifications and associates their names with symbol entries.
    /// </summary>
    /// <param name="symbolTableStack">The symbol-table stack that owns the outermost scope.</param>
    private void InitializeTypes(ISymbolTableStack symbolTableStack)
    {
        (IntegerType, IntegerId) = InitializeType(symbolTableStack, "integer");
        (RealType, RealId) = InitializeType(symbolTableStack, "real");
        (BooleanType, BooleanId) = InitializeType(symbolTableStack, "boolean");
        (CharType, CharId) = InitializeType(symbolTableStack, "char");
        UndefinedType = PascalTypeFactory.CreateType(TypeForm.Scalar);
    }

    /// <summary>
    /// Creates one predefined scalar type and enters its name into the current symbol table.
    /// </summary>
    /// <param name="symbolTableStack">The stack that owns the current symbol table.</param>
    /// <param name="name">The predefined Pascal type name.</param>
    /// <returns>The created type specification and its symbol-table entry.</returns>
    private static (ITypeSpec Type, ISymbolTableEntry Entry) InitializeType(
        ISymbolTableStack symbolTableStack,
        string name
    )
    {
        ISymbolTableEntry entry = symbolTableStack.EnterLocal(name);
        ITypeSpec type = PascalTypeFactory.CreateType(TypeForm.Scalar);

        entry[PascalSymbolTableKey.Definition] = Definition.Type;
        entry[PascalSymbolTableKey.TypeSpec] = type;
        type.Identifier = entry;

        return (type, entry);
    }

    private void InitializeConstants(ISymbolTableStack symbolTableStack)
    {
        FalseId = InitializeBooleanConstant(symbolTableStack, "false", value: 0);
        TrueId = InitializeBooleanConstant(symbolTableStack, "true", value: 1);
        BooleanType[TypeKey.EnumerationConstants] = new List<ISymbolTableEntry>
        {
            FalseId,
            TrueId
        };
    }

    /// <summary>
    /// Creates and enters one Boolean enumeration constant.
    /// </summary>
    /// <param name="symbolTableStack">The stack that owns the current symbol table.</param>
    /// <param name="name">The Boolean constant name.</param>
    /// <param name="value">The ordinal value of the Boolean constant.</param>
    /// <returns>The created symbol-table entry.</returns>
    private ISymbolTableEntry InitializeBooleanConstant(
        ISymbolTableStack symbolTableStack,
        string name,
        int value
    )
    {
        ISymbolTableEntry entry = symbolTableStack.EnterLocal(name);
        entry[PascalSymbolTableKey.Definition] = Definition.EnumerationConstant;
        entry[PascalSymbolTableKey.TypeSpec] = BooleanType;
        entry[SymbolTableKey.ConstantValue] = value;

        return entry;
    }
}
