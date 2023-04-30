using Patterns;
using PatternsUI.MVVM;
using PatternsUI.View;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// Handles logic related to logging in
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        private string _username;
        private bool _isError = false;

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

        /// <summary>
        /// Submits the entered username with the provided password. If the credentials are accepted,
        /// this method will navigate to the next view.
        /// </summary>
        /// <param name="password">The password to submit along with the username</param>
        public void SubmitUsernameAndPassword(object? password)
        {
            IsError = false;
            if (password is string pw && Coordinator.Instance.UserManager.PerformLogin(_username, pw))
            {
                Navigate(typeof(DataRecordsView));
            }
            else
            {
                IsError = true;
            }
        }

        #endregion
    }
}
