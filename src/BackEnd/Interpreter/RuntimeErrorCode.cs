namespace Backend.Interpreter;

/// <summary>
/// Identifies errors that can occur while executing Pascal code.
/// </summary>
public enum RuntimeErrorCode
{
    /// <summary>Indicates use of an uninitialized value.</summary>
    UninitializedValue,
    /// <summary>Indicates a value outside its permitted range.</summary>
    ValueRange,
    /// <summary>Indicates an invalid CASE-expression value.</summary>
    InvalidCaseExpressionValue,
    /// <summary>Indicates division by zero.</summary>
    DivisionByZero,
    /// <summary>Indicates an invalid standard-function argument.</summary>
    InvalidStandardFunctionArgument,
    /// <summary>Indicates invalid input.</summary>
    InvalidInput,
    /// <summary>Indicates a runtime stack overflow.</summary>
    StackOverflow,
    /// <summary>Indicates execution of an unimplemented runtime feature.</summary>
    UnimplementedFeature
}

/// <summary>
/// Provides metadata and diagnostic text for <see cref="RuntimeErrorCode"/> values.
/// </summary>
public static class RuntimeErrorCodeExtensions
{
    extension(RuntimeErrorCode errorCode)
    {
        /// <summary>
        /// Gets the process exit status associated with an error code.
        /// </summary>
        public int Status => (int)errorCode;

        /// <summary>
        /// Gets the diagnostic message for an error code.
        /// </summary>
        public string Message => MessageFor(errorCode);
    }

    private static string MessageFor(RuntimeErrorCode code)
    {
        return code switch
        {
            RuntimeErrorCode.UninitializedValue => "Uninitialized value",
            RuntimeErrorCode.ValueRange => "Value out of range",
            RuntimeErrorCode.InvalidCaseExpressionValue => "Invalid CASE expression value",
            RuntimeErrorCode.DivisionByZero => "Division by zero",
            RuntimeErrorCode.InvalidStandardFunctionArgument => "Invalid standard function argument",
            RuntimeErrorCode.InvalidInput => "Invalid input",
            RuntimeErrorCode.StackOverflow => "Runtime stack overflow",
            RuntimeErrorCode.UnimplementedFeature => "Unimplemented runtime feature",
            _ => code.ToString()
        };
    }
}
