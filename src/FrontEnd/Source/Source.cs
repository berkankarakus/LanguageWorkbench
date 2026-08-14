using Messaging;

namespace FrontEnd.Source;

/// <summary>
/// Represents the source program being scanned.
/// </summary>
public sealed class Source(TextReader reader) : IMessageProducer, IDisposable
{
    public const char EndOfLine = '\n';
    public const char EndOfFile = '\0';

    private static readonly MessageHandler MessageHandler = new();
    private readonly TextReader _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    private string? _line;
    public int LineNumber { get; private set; }
    public int CurrentPosition { get; private set; } = -2;

    public char CurrentChar()
    {
        // First time?
        if (CurrentPosition == -2)
        {
            ReadLine();
            return NextChar();
        }

        if (_line is null)
            return EndOfFile;

        if (CurrentPosition == -1 || CurrentPosition == _line.Length)
            return EndOfLine;

        return _line[CurrentPosition];
    }

    public char NextChar()
    {
        ++CurrentPosition;
        return CurrentChar();
    }

    public char PeekChar()
    {
        CurrentChar();

        if (_line is null)
            return EndOfFile;

        int nextPosition = CurrentPosition + 1;

        return nextPosition < _line.Length
            ? _line[nextPosition]
            : EndOfLine;
    }

    /// <summary>
    /// Read the next source line.
    /// </summary>
    private void ReadLine()
    {
        _line = _reader.ReadLine(); // null when at the end of the source
        CurrentPosition = 0;

        if (_line is not null)
            ++LineNumber;

        // Send a source line message containing the line number and the line text to all the listeners.
        if (_line is not null)
            SendMessage(new Message(MessageType.SourceLine, new object[] { LineNumber, _line }));
    }

    private void Close()
    {
        try
        {
            _reader.Dispose();
        }

        catch (IOException ex)
        {
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// Add a parser message listener.
    /// </summary>
    /// <param name="listener"></param>
    public void AddMessageListener(IMessageListener listener)
    {
        MessageHandler.AddListener(listener);
    }

    /// <summary>
    /// Remove a parser message listener.
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveMessageListener(IMessageListener listener)
    {
        MessageHandler.RemoveListener(listener);
    }

    /// <summary>
    /// Notify listeners after setting the message.
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SendMessage(Message message)
    {
        MessageHandler.SendMessage(message);
    }

    public void Dispose() => Close();
}
