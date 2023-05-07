using CommunityToolkit.Diagnostics;
using Patterns;
using Patterns.Account;
using Patterns.Account.Model;
using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
using PatternsUI.View;
using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// The main view - this houses the other views as content, and is responsible for top level functionality
    /// like managing menu items, or the profile picture. This class also provides a way for other views to navigate
    /// between each other.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private PatternzView _currentPage;

        #endregion

        #region Properties

        public PatternzView CurrentPage { 
            get => _currentPage; 
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public RelayCommand LogoutCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }

        public bool IsMenuVisible { get => CurrentPage.GetType() != typeof(Login); }
        public ImageSource CurrentUserPicture { get; private set; }

        public string CurrentUserName { get; private set; }

        #endregion

        #region Constructors and Methods

        public MainWindowViewModel()
        {
            LogoutCommand = new RelayCommand(RequestLogout, (_) => 
                !Coordinator.Instance.UserManager.CurrentUser.IsAnyUser);
            ExitCommand = new RelayCommand(QuitApplication);
            AboutCommand = new RelayCommand(About);
            CurrentUserPicture = new BitmapImage();

            Coordinator.Instance.UserManager.CurrentUserChanged += OnCurrentUserChanged;

            Messenger.Register<LogoutMessage>(this, OnLogoutMessage);

            NavigateToView(typeof(Login));
        }

        /// <summary>
        /// Updates relevant visual elements when the current user changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnCurrentUserChanged(object? sender, CurrentUserChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewUser.PictureUrl))
            {
                CurrentUserPicture
                    = new BitmapImage(new Uri($"pack://application:,,,/PatternsUI;component/{e.NewUser.PictureUrl}", UriKind.Absolute));
            }
            else
            {
                CurrentUserPicture = new BitmapImage();
            }
            CurrentUserName = e.NewUser.DisplayName;
            NotifyPropertyChanged(nameof(CurrentUserName));
            NotifyPropertyChanged(nameof(CurrentUserPicture));
        }

        /// <summary>
        /// Requests a logout of the current user.
        /// </summary>
        /// <param name="_"></param>
        public void RequestLogout(object? _)
        {
            Action logout = () =>
            {
                Messenger.Send(new LogoutMessage());
            };

            if (CurrentPage.ViewModel != null)
            {
                CurrentPage.ViewModel.RequestExit(logout);
            }
            else
            {
                logout();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs a logout in the user manager, and resets the UI to the login page
        /// </summary>
        /// <param name="message"></param>
        private void OnLogoutMessage(IMessage message)
        {
            Coordinator.Instance.UserManager.PerformLogout();
            Messenger.Send(new ClearUIMessage());
            Messenger.Send(new ClearFocusMessage());
            NavigateToView(typeof(Login), "You have been logged out");
        }

        /// <summary>
        /// Creates and navigates to a new instance of the destination view, 
        /// calling the current view's view model's OnUnloaded in the process
        /// </summary>
        /// <param name="viewType">The type of view to navigate to</param>
        private void NavigateToView(Type viewType, object? context = null)
        {
            PatternzView? nextView = Activator.CreateInstance(viewType) as PatternzView;
            if (nextView != null)
            {
                CurrentPage = nextView;
                if (CurrentPage.ViewModel is INavigator navigator)
                {
                    navigator.NavAction = NavigateToView;
                }
                if (CurrentPage.ViewModel is IViewModel viewModel) 
                {
                    viewModel.ApplyContext(context);
                }
                PrepareMenuItems();
            }
            else
            {
                ThrowHelper.ThrowInvalidOperationException($"Unable to navigate to page, unrecognizable type: {viewType}");
            }
        }

        /// <summary>
        /// Requests to quit the application
        /// </summary>
        /// <param name="_"></param>
        private void QuitApplication(object? _)
        {
            Action exit = () =>
            {
                CurrentPage.ViewModel?.OnUnloaded();
                Application.Current.Shutdown();
            };
            if (CurrentPage.ViewModel != null)
            {
                CurrentPage.ViewModel.RequestExit(exit);
            }
            else 
            {
                exit(); 
            }
        }

        /// <summary>
        /// Displays the About dialog for this application
        /// </summary>
        /// <param name="_"></param>
        private void About(object? _)
        {
            ShowPopupMessage aboutMsg = new("About Patternz", "Patternz is an educational application " +
                "designed to explore and apply various design patterns to create an extensible, robust " +
                "application framework.");
            Messenger.Send(aboutMsg);
        }

        /// <summary>
        /// Gets any menu items from the CurrentPage, assigning to the appropriate menu bucket.
        /// This method also adds a few menu items related to the overall application
        /// </summary>
        private void PrepareMenuItems()
        {
            if (CurrentPage.ViewModel is ViewModelBase viewModel)
            {
                FileMenuItems = viewModel.FileMenuItems ?? new ObservableCollection<MenuItem>();
                EditMenuItems = viewModel.EditMenuItems ?? new ObservableCollection<MenuItem>();
                ViewMenuItems = viewModel.ViewMenuItems ?? new ObservableCollection<MenuItem>();
                HelpMenuItems = viewModel.HelpMenuItems ?? new ObservableCollection<MenuItem>();
            }

            FileMenuItems.Add(new MenuItem() { Header = "Exit", Command = ExitCommand });
            AttachMenuItemNames(FileMenuItems);
            NotifyPropertyChanged(nameof(FileMenuItems));

            AttachMenuItemNames(EditMenuItems);
            NotifyPropertyChanged(nameof(EditMenuItems));

            AttachMenuItemNames(ViewMenuItems);
            NotifyPropertyChanged(nameof(ViewMenuItems));

            HelpMenuItems.Add(new MenuItem() { Header = "About Application", Command = AboutCommand });
            AttachMenuItemNames(HelpMenuItems);
            NotifyPropertyChanged(nameof(HelpMenuItems));

            NotifyPropertyChanged(nameof(IsMenuVisible));
        }

        /// <summary>
        /// Generates names for each menu item
        /// </summary>
        /// <param name="menuItems"></param>
        private void AttachMenuItemNames(ObservableCollection<MenuItem> menuItems)
        {
            foreach (MenuItem menuItem in menuItems) 
            {
                if (string.IsNullOrEmpty(menuItem.Name) && menuItem.Header is string header)
                {
                    menuItem.Name = "mnu" + header.Replace(" ", "");
                }
            }
        }

        #endregion
    }
}
