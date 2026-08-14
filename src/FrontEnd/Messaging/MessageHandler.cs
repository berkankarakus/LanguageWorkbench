namespace FrontEnd.Messaging;

/// <summary>
/// A helper class to which message producer classes delegate the task of maintaining and notifying listeners.
/// </summary>
public class MessageHandler
{
    private Message _message;
    private List<IMessageListener> _listeners;

    public MessageHandler()
    {
        _listeners = new();
    }

    /// <summary>
    /// Add a listener to the listeners list.
    /// </summary>
    /// <param name="listener">the listener to add</param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddListener(IMessageListener listener)
    {
        _listeners.Add(listener);
    }

    /// <summary>
    /// Remove a listener from the listener list.
    /// </summary>
    /// <param name="listener">the listener to remove</param>
    public void RemoveListener(IMessageListener listener)
    {
        _listeners.Remove(listener);
    }

    /// <summary>
    /// Notify listeners after setting the message.
    /// </summary>
    /// <param name="message">the message to set</param>
    public void SendMessage(Message message)
    {
        _message = message;
        NotifyListeners();
    }

    /// <summary>
    /// Notify each listener in the listener list by calling the listener's MessageReceived() method.
    /// </summary>
    private void NotifyListeners()
    {
        foreach (var listener in _listeners)
            listener.MessageReceived(_message);
    }
}