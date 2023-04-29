using Patterns;
using PatternsUI.MVVM;
using PatternsUI.View;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// Handles logic related to logging in
    /// </summary>
    internal class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private bool _isError = false;

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    NotifyPropertyChanged(nameof(Username));
                }
            }
        }

        public string Password
        {
            private get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged(nameof(Password));
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
                    NotifyPropertyChanged(nameof(IsError));
                }
            }
        }

        public RelayCommand SubmitCommand { get; private set; }

        public LoginViewModel()
        {
            _username = string.Empty;
            _password = string.Empty;
            SubmitCommand = new RelayCommand(SubmitUsernameAndPassword, CanSubmitUsernameAndPassword);
        }

        public void SubmitUsernameAndPassword(object? _)
        {
            IsError = false;
            if (GlobalStateSingleton.Instance.PerformLogin(_username, _password))
            {
                Navigate(typeof(DataRecordsView));
            }
            else
            {
                IsError = true;
            }
        }

        public bool CanSubmitUsernameAndPassword(object? _)
        {
            if (string.IsNullOrEmpty(_username) ||  string.IsNullOrEmpty(_password)) 
                return false;
            return true;
        }
    }
}
