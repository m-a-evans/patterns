using Patterns;
using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
using PatternsUI.View;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// Handles logic related to logging in
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        private const string InvalidCredentialsMessage = "Username or password not recognized.";
        private string _username;
        private bool _isError = false;
        private string _message = InvalidCredentialsMessage;

        #endregion

        #region Properties
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsError
        {
            get => _isError;
            set
            {
                if (_isError != value) 
                {
                    _isError = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MessageForUser
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Command to submit username and password
        /// </summary>
        public RelayCommand SubmitCommand { get; private set; }

        #endregion

        #region Constructors and Methods
        public LoginViewModel()
        {
            _username = string.Empty;
            SubmitCommand = new RelayCommand(SubmitUsernameAndPassword);
        }

        public override void OnLoaded()
        {
            Messenger.Register<LogoutMessage>(this, OnLogoutMessage);
        }

        public override void OnUnloaded()
        {
            Messenger.UnregisterAll(this);
        }

        /// <summary>
        /// Submits the entered username with the provided password. If the credentials are accepted,
        /// this method will navigate to the next view.
        /// </summary>
        /// <param name="password">The password to submit along with the username</param>
        public void SubmitUsernameAndPassword(object? _)
        {
            IsError = false;
            Messenger.Send(new RetrievePasswordMessage((password) =>
            {
                var retVal = false;
                if (password is string pw && Coordinator.Instance.UserManager.PerformLogin(_username, pw))
                {
                    Navigate<DataRecordsView>();
                    retVal = true;
                }
                else
                {
                    IsError = true;
                    MessageForUser = InvalidCredentialsMessage;
                }
                return retVal;
            }));
        }

        public override void ApplyContext(object? context = null)
        {
            if (context is string message)
            {
                MessageForUser = message;
                IsError = true;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Displays the reason for logout in the error text
        /// </summary>
        /// <param name="message"></param>
        private void OnLogoutMessage(IMessage message)
        {
            if (message is LogoutMessage logoutMessage)
            {
                IsError = true;
                MessageForUser = logoutMessage.Message;
            }
        }

        #endregion
    }
}
