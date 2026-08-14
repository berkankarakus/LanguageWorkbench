namespace FrontEnd.Messaging;

public interface IMessageListener
{
    /// <summary>
    /// Called to receive a message sent by a message producer.
    /// </summary>
    /// <param name="message">the message that was sent</param>
    public void MessageReceived(Message message);
}
