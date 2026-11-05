using FrontEnd.Pascal.Types.Interfaces;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Types.Implementations;

/// <summary>
/// Stores the form, name, and attributes of a Pascal type specification.
/// </summary>
public sealed class TypeSpec : ITypeSpec
{
    private readonly Predefined? _predefined;

    /// <inheritdoc />
    public ITypeForm Form { get; }

    /// <inheritdoc />
    public ISymbolTableEntry? Identifier { get; set; }

    /// <summary>
    /// Stores form-specific metadata, such as array bounds, element types, and record symbol tables.
    /// </summary>
    private readonly Dictionary<ITypeKey, object?> _attributes = [];

    /// <summary>
    /// Initializes a type specification with its structural form.
    /// </summary>
    /// <param name="form">The structural form of the type.</param>
    public TypeSpec(ITypeForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        Form = form;
    }

    /// <summary>
    /// Initializes the type specification used for a Pascal string literal.
    /// </summary>
    /// <param name="value">The source string literal.</param>
    public TypeSpec(string value, Predefined predefined)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(predefined);

        Form = TypeForm.Array;
        _predefined = predefined;

        TypeSpec indexType = new(TypeForm.Subrange);
        indexType[TypeKey.SubrangeBaseType] = predefined.IntegerType;
        indexType[TypeKey.SubrangeMinValue] = 1;
        indexType[TypeKey.SubrangeMaxValue] = value.Length;

        this[TypeKey.ArrayIndexType] = indexType;
        this[TypeKey.ArrayElementType] = predefined.CharType;
        this[TypeKey.ArrayElementCount] = value.Length;
    }

    /// <inheritdoc />
    public object? this[ITypeKey key]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(key);
            return _attributes.GetValueOrDefault(key);
        }
        set
        {
            ArgumentNullException.ThrowIfNull(key);
            _attributes[key] = value;
        }
    }

    /// <inheritdoc />
    public bool IsPascalString()
    {
        if ((TypeForm)Form != TypeForm.Array)
            return false;

        Predefined predefined = _predefined
            ?? throw new InvalidOperationException("A Pascal string type requires predefined type metadata.");

        TypeSpec elementType = (TypeSpec)this[TypeKey.ArrayElementType]!;
        TypeSpec indexType = (TypeSpec)this[TypeKey.ArrayIndexType]!;

        return
            elementType.BaseType == predefined.CharType &&
            indexType.BaseType == predefined.IntegerType;
    }

    /// <inheritdoc />
    public ITypeSpec? BaseType
    {
        get
        {
            return Form.Equals(TypeForm.Subrange)
                ? this[TypeKey.SubrangeBaseType] as ITypeSpec
                : this;
        }
    }
}
