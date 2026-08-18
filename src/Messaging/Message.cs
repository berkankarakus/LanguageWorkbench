namespace Messaging;

/// <summary>
/// Represents an event exchanged between language-workbench components.
/// </summary>
public sealed class Message(MessageType type, object body)
{
    /// <summary>Gets the category of this message.</summary>
    public MessageType Type { get; } = type;

    /// <summary>Gets the payload associated with <see cref="Type"/>.</summary>
    public object Body { get; } = body;
}

/// <summary>
/// Identifies the event categories exchanged by message producers and listeners.
/// </summary>
public enum MessageType
{
    SourceLine,
    SyntaxError,
    ParserSummary,
    InterpreterSummary,
    CompilerSummary,
    Miscellaneous,
    Token,
    Assign,
    Fetch,
    Breakpoint,
    RuntimeError,
    Call,
    Return
}
