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
        // Recipient is a weak reference to let it be garbage collected,
        // in the case where it forgets to unregister itself from the messenger 
        // but goes out of scope
        public WeakReference<object> Recipient;
        public Action<IMessage> Target;

        public MessageRecipient(object recipient, Action<IMessage> target)
        {
            Recipient = new WeakReference<object>(recipient);
            Target = target;
        }
    }
}
