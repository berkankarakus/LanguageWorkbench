namespace Messaging;

/// <summary>
/// Publishes messages to registered <see cref="IMessageListener"/> instances.
/// </summary>
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
    /// <param name="message">The message to publish to registered listeners.</param>
    public void SendMessage(Message message);
}
