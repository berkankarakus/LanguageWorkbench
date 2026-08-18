using Messaging;

namespace FrontEnd.Input;

/// <summary>
/// Represents the source program being scanned.
/// </summary>
public sealed class SourceReader(TextReader reader) : IMessageProducer, IDisposable
{
    /// <summary>Marks the end of a source line.</summary>
    public const char EndOfLine = '\n';

    /// <summary>Marks the end of the source stream.</summary>
    public const char EndOfFile = '\0';

    private static readonly MessageHandler MessageHandler = new();
    private readonly TextReader _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    private string? _line;
    public int LineNumber { get; private set; }
    public int CurrentPosition { get; private set; } = -2;

    /// <summary>
    /// Gets the current source character without advancing the reader.
    /// </summary>
    /// <returns>The current character, <see cref="EndOfLine"/>, or <see cref="EndOfFile"/>.</returns>
    public char CurrentChar()
    {
        if (CurrentPosition == -2)
            ReadLine();

        if (_line is null)
            return EndOfFile;

        if (CurrentPosition < _line.Length)
            return _line[CurrentPosition];

        return EndOfLine;
    }

    /// <summary>
    /// Advances the reader and returns the new current character.
    /// </summary>
    /// <returns>The next character, <see cref="EndOfLine"/>, or <see cref="EndOfFile"/>.</returns>
    public char NextChar()
    {
        if (_line is null)
            return EndOfFile;

        if (CurrentPosition < _line.Length)
        {
            ++CurrentPosition;
            return CurrentChar();
        }

        ReadLine();
        return CurrentChar();
    }

    /// <summary>
    /// Reads the next source line and publishes a <see cref="MessageType.SourceLine"/> message.
    /// </summary>
    private void ReadLine()
    {
        _line = _reader.ReadLine(); // null when at the end of the source
        CurrentPosition = 0;

        if (_line is not null)
            ++LineNumber;

        // Send a source line message containing the line number and the line text to all the listeners.
        if (_line is not null)
            SendMessage(new Message(MessageType.SourceLine, new object?[] { LineNumber, _line }));
    }

    /// <summary>
    /// Returns the character after the current character without advancing the reader.
    /// </summary>
    /// <returns>The next character, <see cref="EndOfLine"/>, or <see cref="EndOfFile"/>.</returns>
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
    /// Adds a source-message listener.
    /// </summary>
    /// <param name="listener">The listener to add.</param>
    public void AddMessageListener(IMessageListener listener)
    {
        MessageHandler.AddListener(listener);
    }

    /// <summary>
    /// Removes a source-message listener.
    /// </summary>
    /// <param name="listener">The listener to remove.</param>
    public void RemoveMessageListener(IMessageListener listener)
    {
        MessageHandler.RemoveListener(listener);
    }

    /// <summary>
    /// Publishes a source message to registered listeners.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    public void SendMessage(Message message)
    {
        MessageHandler.SendMessage(message);
    }

    public void Dispose() => Close();
}
