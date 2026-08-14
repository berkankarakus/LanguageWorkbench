namespace Messaging;

public interface IMessageProducer
{
    /// <summary>
    /// Add a listener to the listeners list.
    /// </summary>
    /// <param name="listener">The listener to add</param>
    public void AddMessageListener(IMessageListener listener);

    /// <summary>
    /// Remove a listener from the listener list.
    /// </summary>
    /// <param name="listener">The listener to remove</param>
    public void RemoveMessageListener(IMessageListener listener);

    /// <summary>
    /// Notify listeners after setting the message.
    /// </summary>
    /// <param name="message"></param>
    public void SendMessage(Message message);
}
