using System;

namespace PatternsUI.MVVM.Messages
{
    public class RetrievePasswordMessage : IMessage
    {
        public Func<string?, bool> Callback { get; private set; }

        public RetrievePasswordMessage(Func<string?, bool> callback)
        {
            Callback = callback;
        }
    }
}
