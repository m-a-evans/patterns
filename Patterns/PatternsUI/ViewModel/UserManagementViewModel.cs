using CommunityToolkit.Diagnostics;
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
using System.Windows.Threading;

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
        private IUserAccount _userAccounts;

        private Task? _writingUserStoreTask;

        private const string AdminUrl = @"/Resources/Images/Admin.png";
        private const string UserUrl = @"/Resources/Images/User.png";

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
            AddUserCommand = new RelayCommand(TurnOnAddUserMode);
            RemoveUserCommand = new RelayCommand(RemoveUser, CanRemoveUser);
            SelectIsAdminCommand = new RelayCommand(SelectIsAdmin);

            UserPermissions = new ObservableCollection<UserPermission>()
            {
                new UserPermission(Permission.AddUser),
                new UserPermission(Permission.RemoveUser),
                new UserPermission(Permission.UpdateUser),
                new UserPermission(Permission.WriteAccess),
                new UserPermission(Permission.ReadAccess),
            };

            _userAccounts = Coordinator.Instance.GetUserAccountManager();

            _ = RefreshUserListAsync();

            PrepareMenuItems();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Kicks of the save process for the user
        /// </summary>
        /// <param name="_"></param>
        private void Save(object? _)
        {
            if (Coordinator.Instance.UserManager.CurrentUser.Username == Username)
            {
                Messenger.Send(new ShowYesNoPopupMessage("Confirm Edit", "Editing the current user will log you out. Are you sure?",
                    (confirmed) =>
                    {
                        if (confirmed)
                        {
                            Messenger.Send(new RetrievePasswordMessage(LogoutAfterUpsertUserAction));
                        }
                    }));
            }
            else
            {
                Messenger.Send(new RetrievePasswordMessage(UpsertUserAction));
            }
        }

        /// <summary>
        /// Sends a logout message if an upsert action succeeds
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool LogoutAfterUpsertUserAction(string? password)
        {
            bool upsertSuccess = UpsertUserAction(password);
            if (upsertSuccess) 
            {
                Messenger.Send(new LogoutMessage());
            }
            return upsertSuccess;
        }

        /// <summary>
        /// Attempts to update the user store to include a new user or to update an existing user
        /// </summary>
        /// <param name="password">Optional for updates, required for new. </param>
        private bool UpsertUserAction(string? password)
        {
            const string unableToSaveMsg = "Unable to Save";
            bool doneEditing = false;
            bool retVal = false;
            IPatternzUser? user;
            try
            {
                if (IsAddingUser)
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        ThrowHelper.ThrowInvalidOperationException("Password must be filled out");
                    }
                    if (!_userAccounts.TryGetUser(Username, out user))
                    {
                        user = _userAccounts.CreateUser(Username, password, DisplayName, IsUserAdmin ? AdminUrl : UserUrl, ReadSelectedPermissions());
                    }
                    else
                    {
                        ThrowHelper.ThrowInvalidOperationException($"Username {user?.Username} already taken");
                    }
                }
                else if (IsEditingUser)
                {
                    if (_userAccounts.TryGetUser(Username, out user))
                    {
                        user!.PictureUrl = IsUserAdmin ? AdminUrl : UserUrl;
                        user.DisplayName = DisplayName;
                        user.Permissions = ReadSelectedPermissions();
                        password = string.IsNullOrWhiteSpace(password) ? null : password;
                        _userAccounts.UpdateUser(user, password);
                    }
                    else
                    {
                        ThrowHelper.ThrowInvalidOperationException($"User {user?.Username} does not exist");
                    }
                }
                doneEditing = true;
                _writingUserStoreTask = _userAccounts.TryWriteUsersToStoreAsync();
                Dispatcher.CurrentDispatcher.InvokeAsync(async () => { await _writingUserStoreTask; });
                retVal = true;
            }
            catch (UnauthorizedAccessException)
            {
                Messenger.Send(new ShowPopupMessage("Unable to Remove User", $"You do not have sufficent authorization to {(IsAddingUser ? "add" : "edit")} this user."));
            }
            catch (InvalidOperationException ioex)
            {
                Messenger.Send(new ShowPopupMessage(unableToSaveMsg, ioex.Message));
            }

            if (doneEditing)
            {
                IsAddingUser = false;
                IsEditingUser = false;
                _ = RefreshUserListAsync();
                Messenger.Send(new ClearFocusMessage());
                ClearUserEntries();
            }
            return retVal;
        }

        /// <summary>
        /// Cancels adding or editing a user
        /// </summary>
        /// <param name="_"></param>
        private void Cancel(object? _)
        {
            ClearUserEntries();
        }

        /// <summary>
        /// Sets all of the User Entry fields to blank/false/none
        /// </summary>
        private void ClearUserEntries()
        {
            IsAddingUser = false;
            IsEditingUser = false;
            Username = string.Empty;
            IsUserAdmin = false;
            DisplayName = string.Empty;
            SelectAllPermissions(false);
            
            Messenger.Send(new ClearUIMessage());

            NotifyUserGridProperties();
        }

        /// <summary>
        /// Updates the User Entry fields to match the data of
        /// the provided PatternzUser
        /// </summary>
        /// <param name="param">The PatternzUser for whom the fields will be set from</param>
        private void SelectUser(object? param)
        {
            if (param is IPatternzUser user)
            {
                ClearUserEntries();
                Username = user.Username;
                DisplayName = user.DisplayName;
                SelectPermissions(user);
                if (user.PictureUrl.Contains("Admin"))
                {
                    IsUserAdmin = true;
                }
                IsEditingUser = true;
                IsAddingUser = false;
                NotifyUserGridProperties();
            }
        }

        /// <summary>
        /// Raises the NotifyPropertyChanged event on all of the User Entry fields
        /// </summary>
        private void NotifyUserGridProperties()
        {
            NotifyPropertyChanged(nameof(IsAddingUser));
            NotifyPropertyChanged(nameof(IsUserAdmin));
            NotifyPropertyChanged(nameof(IsEditingUser));
            NotifyPropertyChanged(nameof(DisplayName));
            NotifyPropertyChanged(nameof(Username));
            NotifyPropertyChanged(nameof(UserPermissions));
            NotifyPropertyChanged(nameof(IsAddingOrEditingUser));
        }

        /// <summary>
        /// Selects or deselects the permissions for the user depending on if they are admin or not
        /// </summary>
        /// <param name="param">Boolean indicating the user entry fields are for an admin</param>
        private void SelectIsAdmin(object? param)
        {
            SelectAllPermissions(IsUserAdmin);
            NotifyUserGridProperties();
        }

        /// <summary>
        /// Sets the permissions property on the UserPermission to match what permissions the PatternzUser has
        /// </summary>
        /// <param name="user">The user to set the permissions on</param>
        private void SelectPermissions(IPatternzUser user)
        {
            SelectAllPermissions(false);
            foreach (UserPermission permission in UserPermissions)
            {
                permission.IsEnabled = (user.Permissions & permission.Permission) == permission.Permission;
            }
        }

        /// <summary>
        /// Returns a Permission bit mask with the permissions set to the corresponding UserPermission selections
        /// </summary>
        private Permission ReadSelectedPermissions()
        {
            Permission result = Permission.None;
            foreach (UserPermission permission in UserPermissions)
            {
                if (permission.IsEnabled)
                {
                    result |= permission.Permission;
                }
            }
            return result;
        }

        /// <summary>
        /// Sets IsEnabled to the parameter for every property in the UserPermission list
        /// </summary>
        /// <param name="on">The boolean flag to set all UserPermissions to</param>
        private void SelectAllPermissions(bool on)
        {
            foreach (UserPermission permission in UserPermissions)
            {
                permission.IsEnabled = on;
            }
        }

        /// <summary>
        /// Attempts to remove a user from the user store
        /// </summary>
        /// <param name="_"></param>
        private void RemoveUser(object? _)
        {
            if (Coordinator.Instance.UserManager.CurrentUser.Username == Username)
            {
                Messenger.Send(new ShowYesNoPopupMessage("Confirm User Removal", "Deleting the current user will log you out. Are you sure?", 
                    (confirmed) =>
                    {
                        if (confirmed)
                        {
                            if (RemoveUserAction())
                            {
                                Messenger.Send(new LogoutMessage());
                            }
                        }
                    }));
            }
            else
            {
                _ = RemoveUserAction();
            }
        }

        /// <summary>
        /// Removes the user currently being edited
        /// </summary>
        /// <returns></returns>
        private bool RemoveUserAction()
        {
            bool retVal = false;
            IPatternzUser? user;
            try
            {
                if (_userAccounts.TryGetUser(Username, out user) && user != null)
                {
                    _userAccounts.DeleteUser(user);
                    _writingUserStoreTask = _userAccounts.TryWriteUsersToStoreAsync();
                    Dispatcher.CurrentDispatcher.InvokeAsync(async () => { await _writingUserStoreTask; });
                    ClearUserEntries();
                    Messenger.Send(new ClearFocusMessage());
                    retVal = true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Messenger.Send(new ShowPopupMessage("Unable to Remove User", "You do not have sufficent authorization to remove this user."));
            }
            catch (InvalidOperationException ioex)
            {
                Messenger.Send(new ShowPopupMessage("Unable to Remove User", ioex.Message));
            }

            _ = RefreshUserListAsync();
            return retVal;
        }
        
        /// <summary>
        /// Returns true if the page is in Edit mode (meaning a user is selected already)
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        private bool CanRemoveUser(object? _)
        {
            return IsEditingUser;
        }

        /// <summary>
        /// Updates the properties related to adding a user mode
        /// </summary>
        /// <param name="_"></param>
        private void TurnOnAddUserMode(object? _) 
        {
            IsAddingUser = true;
            IsEditingUser = false;
            NotifyPropertyChanged(nameof(IsAddingOrEditingUser));
        }

        /// <summary>
        /// Gets the list of users from the user store
        /// </summary>
        /// <returns>The Task encapsulating refreshing the user list</returns>
        private async Task RefreshUserListAsync()
        {
            try
            {
                if (_writingUserStoreTask != null && !_writingUserStoreTask.IsCompleted) 
                {
                    await _writingUserStoreTask;
                }
                
                bool couldRead = await _userAccounts.TryReadUsersFromStoreAsync();
                if (!couldRead) 
                {
                    Messenger.Send(new ShowPopupMessage("Error", "Unable to read user store."));
                    return;
                }
                _userList = new ObservableCollection<IPatternzUser>();
                foreach(IPatternzUser user in _userAccounts.GetUserListCopy())
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

        /// <summary>
        /// Navigates to the DataRecords Page
        /// </summary>
        /// <param name="_"></param>
        private void NavigateToDataRecords(object? _)
        {
            Navigate<DataRecordsView>();
        }

        /// <summary>
        /// Shows the about dialog for this page
        /// </summary>
        /// <param name="_"></param>
        private void ShowAbout(object? _) 
        {
            Messenger.Send(new ShowPopupMessage("About User Management", "The User Management page is where you can add, update or remove users." +
                "\n\nThis page makes use of the proxy pattern to ensure the current user has the correct permissions to perform these actions."));
        }

        /// <summary>
        /// Adds the menu items for this page
        /// </summary>
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
