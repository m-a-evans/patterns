using Microsoft.Windows.Themes;
using Patterns;
using Patterns.Account;
using Patterns.Account.Model;
using PatternsUI.Model;
using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
using PatternsUI.View;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// Manages users, allowing adding, updating, and removing from the store
    /// </summary>
    public class UserManagementViewModel : ViewModelBase
    {
        #region Fields

        private bool _isUserAdmin;
        private ObservableCollection<IPatternzUser> _userList;
        private ObservableCollection<UserPermission> _userPermissions;
        private string _username;
        private string _displayName;
        private bool _isAddingUser;
        private bool _isEditingUser;

        #endregion

        #region Properties

        public RelayCommand NavigateToDataRecordsCommand { get; private set; }
        public RelayCommand ShowAboutCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand AddUserCommand { get; private set; }
        public RelayCommand RemoveUserCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand SelectUserCommand { get; private set; }
        public RelayCommand SelectIsAdminCommand { get; private set; }

        public bool IsUserAdmin
        {
            get => _isUserAdmin;
            set
            {
                if (_isUserAdmin != value) 
                {
                    _isUserAdmin = value;
                    NotifyPropertyChanged();
                }
            }
        }

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

        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsAddingUser
        {
            get => _isAddingUser;
            set
            {
                if (_isAddingUser != value)
                {
                    _isAddingUser = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsEditingUser
        {
            get => _isEditingUser;
            set
            {
                if (_isEditingUser != value)
                {
                    _isEditingUser = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsAddingOrEditingUser
        {
            get => _isAddingUser || _isEditingUser;
        }

        public ObservableCollection<IPatternzUser> UserList
        {
            get => _userList;
            set
            {
                if (_userList != value) 
                {
                    _userList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<UserPermission> UserPermissions
        {
            get => _userPermissions;
            set
            {
                if (_userPermissions != value)
                {
                    _userPermissions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors and Methods

        public UserManagementViewModel() 
        {
            NavigateToDataRecordsCommand = new RelayCommand(NavigateToDataRecords);
            ShowAboutCommand = new RelayCommand(ShowAbout);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            SelectUserCommand = new RelayCommand(SelectUser);
            AddUserCommand = new RelayCommand(AddUser);
            RemoveUserCommand = new RelayCommand(RemoveUser);
            SelectIsAdminCommand = new RelayCommand(SelectIsAdmin);

            _ = GetUserList();

            UserPermissions = new ObservableCollection<UserPermission>()
            {
                new UserPermission(Permission.AddUser),
                new UserPermission(Permission.RemoveUser),
                new UserPermission(Permission.UpdateUser),
                new UserPermission(Permission.WriteAccess),
                new UserPermission(Permission.ReadAccess),
            };

            PrepareMenuItems();
        }

        #endregion

        #region Private Methods

        private void Save(object? _)
        {

        }

        private void Cancel(object? _)
        {

        }

        private void SelectUser(object? param)
        {
            if (param is IPatternzUser user)
            {
                Username = user.Username;
                DisplayName = user.DisplayName;
                SelectPermissions(user);
                if (user.PictureUrl.Contains("admin"))
                {
                    IsUserAdmin = true;
                }
                IsEditingUser = true;
                NotifyUserGridChanged();
            }
        }

        private void NotifyUserGridChanged()
        {
            NotifyPropertyChanged(nameof(IsAddingUser));
            NotifyPropertyChanged(nameof(IsUserAdmin));
            NotifyPropertyChanged(nameof(DisplayName));
            NotifyPropertyChanged(nameof(Username));
            NotifyPropertyChanged(nameof(UserPermissions));
            NotifyPropertyChanged(nameof(IsAddingOrEditingUser));
            NotifyPropertyChanged(nameof(IsEditingUser));
        }

        private void SelectIsAdmin(object? param)
        {
            if (param is bool isChecked)
            {
                SelectAllPermissions(isChecked);
                NotifyUserGridChanged();
            }
        }

        private void SelectPermissions(IPatternzUser user)
        {
            foreach (UserPermission permission in UserPermissions)
            {
                permission.IsEnabled = (user.Permissions & permission.Permission) == permission.Permission;
            }
        }

        private void SelectAllPermissions(bool on)
        {
            foreach (UserPermission permission in UserPermissions)
            {
                permission.IsEnabled = on;
            }
        }

        private void RemoveUser(object? _)
        {

        }

        private void AddUser(object? _) 
        {

        }

        private async Task GetUserList()
        {
            try
            {
                IUserAccount accounts = Coordinator.Instance.GetUserAccountManager();
                bool couldRead = await accounts.TryReadUsersFromStoreAsync();
                if (!couldRead) 
                {
                    Messenger.Send(new ShowPopupMessage("Error", "Unable to read user store."));
                    return;
                }
                _userList = new ObservableCollection<IPatternzUser>();
                foreach(IPatternzUser user in accounts.GetUserListCopy())
                {
                    _userList.Add(user);
                }
                NotifyPropertyChanged(nameof(UserList));
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine($"The current user, {Coordinator.Instance.UserManager.CurrentUser.Username} is not authorized to read users");
            }
        }
        private void NavigateToDataRecords(object? _)
        {
            Navigate(typeof(DataRecordsView));
        }

        private void ShowAbout(object? _) 
        {
            Messenger.Send(new ShowPopupMessage("About User Management", "The User Management page is where you can add, update or remove users." +
                "\n\nThis page makes use of the proxy pattern to ensure the current user has the correct permissions to perform these actions."));
        }

        private void PrepareMenuItems()
        {
            ViewMenuItems = new()
            {
                new MenuItem() { Header = "Data Records", Command = NavigateToDataRecordsCommand }
            };
            HelpMenuItems = new()
            {
                new MenuItem() { Header = "About This Page", Command = ShowAboutCommand }
            };
        }

        #endregion
    }
}
