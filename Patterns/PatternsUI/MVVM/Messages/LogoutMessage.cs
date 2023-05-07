namespace PatternsUI.MVVM.Messages
{
    public class LogoutMessage : IMessage
    {
        public string Message { get; private set; }

        public LogoutMessage(string message = "You have been logged out.") 
        {
            Message = message;
        }
    }
}
