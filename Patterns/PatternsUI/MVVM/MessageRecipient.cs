using PatternsUI.MVVM.Messages;
using System;

namespace PatternsUI.MVVM
{
    /// <summary>
    /// A message recipient, which associates specific objects and the actions they wish to perform upon message arrival
    /// </summary>
    public class MessageRecipient
    {
        public Guid Id { get; } = Guid.NewGuid();
        public WeakReference<object> Recipient;
        public WeakReference<Action<IMessage>> Target;

        public MessageRecipient(object recipient, Action<IMessage> target)
        {
            Recipient = new WeakReference<object>(recipient);
            Target = new WeakReference<Action<IMessage>>(target);
        }
    }
}
