using Messaging;

namespace LanguageWorkbench.Tests;

internal sealed class TestMessageListener : IMessageListener
{
    public List<Message> Messages { get; } = [];

    public void MessageReceived(Message message)
    {
        Messages.Add(message);
    }
}
