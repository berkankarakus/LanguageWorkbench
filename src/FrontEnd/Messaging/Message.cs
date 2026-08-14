namespace FrontEnd.Messaging;

public class Message(MessageType type, object body);

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