namespace Messaging;

public sealed class Message(MessageType type, object body)
{
    public MessageType Type { get; } = type;

    public object Body { get; } = body;
}

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
