using FrontEnd.Pascal.Types.Implementations;
using FrontEnd.Pascal.Types.Interfaces;

namespace FrontEnd.Pascal.Types.Factory;

/// <summary>
/// Creates Pascal type specifications.
/// </summary>
public static class PascalTypeFactory
{
    /// <summary>
    /// Creates a type specification with the supplied structural form.
    /// </summary>
    /// <param name="form">The structural form of the type.</param>
    /// <returns>The new Pascal type specification.</returns>
    public static ITypeSpec CreateType(ITypeForm form)
    {
        return new TypeSpec(form);
    }

    /// <summary>
    /// Creates the array type specification used for a Pascal string literal.
    /// </summary>
    /// <param name="value">The source string literal.</param>
    /// <param name="predefined">The predefined types owned by the current translation.</param>
    /// <returns>The new Pascal string type specification.</returns>
    public static ITypeSpec CreateStringType(string value, Predefined predefined)
    {
        return new TypeSpec(value, predefined);
    }
}
