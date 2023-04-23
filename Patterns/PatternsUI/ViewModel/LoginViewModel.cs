using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Patterns;
using PatternsUI.MVVM;
using PatternsUI.View;

namespace PatternsUI.ViewModel
{
    internal class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
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

        public RelayCommand SubmitCommand { get; private set; }

        public LoginViewModel()
        {
            _username = string.Empty;
            _password = string.Empty;
            SubmitCommand = new RelayCommand(SubmitUsernameAndPassword, CanSubmitUsernameAndPassword);
        }

        public void SubmitUsernameAndPassword(object? _)
        {
            if (GlobalStateSingleton.Instance.PerformLogin(_username, _password))
            {
                Navigate(typeof(DataRecordsView));
            }
            else
            {
                // display error
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
