using Backend.CodeGeneration;
using Backend.Interpreter;

namespace Backend;

/// <summary>
/// Creates the back-end processor for the requested operation.
/// </summary>
public static class BackendFactory
{
    /// <summary>
    /// Creates a compiler or interpreter back end.
    /// </summary>
    /// <param name="operation">The requested operation.</param>
    /// <returns>The processor for the requested operation.</returns>
    public static BackendProcessor Create(string operation)
    {
        return operation switch
        {
            "compile" => new CodeGenerator(),
            "execute" => new Executor(),
            _ => throw new ArgumentException("Invalid operation", nameof(operation))
        };
    }
}
