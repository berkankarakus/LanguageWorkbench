namespace Messaging;

/// <summary>
/// Represents an event exchanged between language-workbench components.
/// </summary>
public sealed class Message(MessageType type, object? body)
{
    /// <summary>Gets the category of this message.</summary>
    public MessageType Type { get; } = type;

    /// <summary>Gets the payload associated with <see cref="Type"/>.</summary>
    public object? Body { get; } = body;
}

/// <summary>
/// Identifies the event categories exchanged by message producers and listeners.
/// </summary>
public enum MessageType
{
    /// <summary>A source line was read.</summary>
    SourceLine,
    /// <summary>A syntax error was detected.</summary>
    SyntaxError,
    /// <summary>Parsing completed and produced a summary.</summary>
    ParserSummary,
    /// <summary>Interpretation completed and produced a summary.</summary>
    InterpreterSummary,
    /// <summary>Compilation completed and produced a summary.</summary>
    CompilerSummary,
    /// <summary>A miscellaneous message was produced.</summary>
    Miscellaneous,
    /// <summary>A lexical token was produced.</summary>
    Token,
    /// <summary>A value was assigned.</summary>
    Assign,
    /// <summary>A value was fetched.</summary>
    Fetch,
    /// <summary>A breakpoint was reached.</summary>
    Breakpoint,
    /// <summary>A runtime error was detected.</summary>
    RuntimeError,
    /// <summary>A routine call was entered.</summary>
    Call,
    /// <summary>A routine returned.</summary>
    Return
}
