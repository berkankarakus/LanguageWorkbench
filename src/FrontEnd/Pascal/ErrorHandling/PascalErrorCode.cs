namespace FrontEnd.Pascal.ErrorHandling;

/// <summary>
/// Identifies syntax errors that can occur while parsing Pascal source code.
/// </summary>
public enum PascalErrorCode
{
    AlreadyForwarded,
    IdentifierRedefined,
    IdentifierUndefined,
    IncompatibleAssignment,
    IncompatibleTypes,
    InvalidAssignment,
    InvalidCharacter,
    InvalidConstant,
    InvalidExponent,
    InvalidExpression,
    InvalidField,
    InvalidFraction,
    InvalidIdentifierUsage,
    InvalidIndexType,
    InvalidNumber,
    InvalidStatement,
    InvalidSubrangeType,
    InvalidTarget,
    InvalidType,
    InvalidVarParameter,
    MinGtMax,
    MissingBegin,
    MissingColon,
    MissingColonEquals,
    MissingComma,
    MissingConstant,
    MissingDo,
    MissingDotDot,
    MissingEnd,
    MissingEquals,
    MissingForControl,
    MissingIdentifier,
    MissingLeftBracket,
    MissingOf,
    MissingPeriod,
    MissingProgram,
    MissingRightBracket,
    MissingRightParen,
    MissingSemicolon,
    MissingThen,
    MissingToDownTo,
    MissingUntil,
    MissingVariable,
    CaseConstantReused,
    NotConstantIdentifier,
    NotRecordVariable,
    NotTypeIdentifier,
    RangeInteger,
    RangeReal,
    StackOverflow,
    TooManyLevels,
    TooManySubscripts,
    UnexpectedEof,
    UnexpectedToken,
    Unimplemented,
    Unrecognizable,
    WrongNumberOfParameters,

    IoError = -101,
    TooManyErrors = -102
}

/// <summary>
/// Provides human-readable descriptions for <see cref="PascalErrorCode"/> values.
/// </summary>
public static class PascalErrorCodeExtensions
{
    extension(PascalErrorCode errorCode)
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

    private static string MessageFor(PascalErrorCode code)
    {
        return code switch
        {
            PascalErrorCode.AlreadyForwarded => "Already specified in FORWARD",
            PascalErrorCode.IdentifierRedefined => "Redefined identifier",
            PascalErrorCode.IdentifierUndefined => "Undefined identifier",
            PascalErrorCode.IncompatibleAssignment => "Incompatible assignment",
            PascalErrorCode.IncompatibleTypes => "Incompatible types",
            PascalErrorCode.InvalidAssignment => "Invalid assignment statement",
            PascalErrorCode.InvalidCharacter => "Invalid character",
            PascalErrorCode.InvalidConstant => "Invalid constant",
            PascalErrorCode.InvalidExponent => "Invalid exponent",
            PascalErrorCode.InvalidExpression => "Invalid expression",
            PascalErrorCode.InvalidField => "Invalid field",
            PascalErrorCode.InvalidFraction => "Invalid fraction",
            PascalErrorCode.InvalidIdentifierUsage => "Invalid identifier usage",
            PascalErrorCode.InvalidIndexType => "Invalid index type",
            PascalErrorCode.InvalidNumber => "Invalid number",
            PascalErrorCode.InvalidStatement => "Invalid statement",
            PascalErrorCode.InvalidSubrangeType => "Invalid subrange type",
            PascalErrorCode.InvalidTarget => "Invalid assignment target",
            PascalErrorCode.InvalidType => "Invalid type",
            PascalErrorCode.InvalidVarParameter => "Invalid VAR parameter",
            PascalErrorCode.MinGtMax => "Minimum limit greater than maximum limit",
            PascalErrorCode.MissingBegin => "Missing BEGIN",
            PascalErrorCode.MissingColon => "Missing :",
            PascalErrorCode.MissingColonEquals => "Missing :=",
            PascalErrorCode.MissingComma => "Missing ,",
            PascalErrorCode.MissingConstant => "Missing constant",
            PascalErrorCode.MissingDo => "Missing DO",
            PascalErrorCode.MissingDotDot => "Missing ..",
            PascalErrorCode.MissingEnd => "Missing END",
            PascalErrorCode.MissingEquals => "Missing =",
            PascalErrorCode.MissingForControl => "Invalid FOR control variable",
            PascalErrorCode.MissingIdentifier => "Missing identifier",
            PascalErrorCode.MissingLeftBracket => "Missing [",
            PascalErrorCode.MissingOf => "Missing OF",
            PascalErrorCode.MissingPeriod => "Missing .",
            PascalErrorCode.MissingProgram => "Missing PROGRAM",
            PascalErrorCode.MissingRightBracket => "Missing ]",
            PascalErrorCode.MissingRightParen => "Missing )",
            PascalErrorCode.MissingSemicolon => "Missing ;",
            PascalErrorCode.MissingThen => "Missing THEN",
            PascalErrorCode.MissingToDownTo => "Missing TO or DOWNTO",
            PascalErrorCode.MissingUntil => "Missing UNTIL",
            PascalErrorCode.MissingVariable => "Missing variable",
            PascalErrorCode.CaseConstantReused => "CASE constant reused",
            PascalErrorCode.NotConstantIdentifier => "Not a constant identifier",
            PascalErrorCode.NotRecordVariable => "Not a record variable",
            PascalErrorCode.NotTypeIdentifier => "Not a type identifier",
            PascalErrorCode.RangeInteger => "Integer literal out of range",
            PascalErrorCode.RangeReal => "Real literal out of range",
            PascalErrorCode.StackOverflow => "Stack overflow",
            PascalErrorCode.TooManyLevels => "Nesting level too deep",
            PascalErrorCode.TooManySubscripts => "Too many subscripts",
            PascalErrorCode.UnexpectedEof => "Unexpected end of file",
            PascalErrorCode.UnexpectedToken => "Unexpected token",
            PascalErrorCode.Unimplemented => "Unimplemented feature",
            PascalErrorCode.Unrecognizable => "Unrecognizable input",
            PascalErrorCode.WrongNumberOfParameters => "Wrong number of actual parameters",

            PascalErrorCode.TooManyErrors => "Too many syntax errors",
            PascalErrorCode.IoError => "I/O error",
            _ => "Unknown Pascal error"
        };
    }
}
