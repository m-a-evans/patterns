using CommunityToolkit.Diagnostics;
using Patterns;
using Patterns.Account;
using Patterns.Account.Model;
using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
using PatternsUI.View;
using System;
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
            LogoutCommand = new RelayCommand(Logout, (_) => 
                !Coordinator.Instance.UserManager.CurrentUser.IsAnyUser);
            ExitCommand = new RelayCommand(QuitApplication);
            AboutCommand = new RelayCommand(About);
            CurrentUserPicture = new BitmapImage();

            Coordinator.Instance.UserManager.CurrentUserChanged += OnCurrentUserChanged;

            NavigateToView(typeof(Login));
        }

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

        public void Logout(object? _)
        {
            Action logout = () =>
            {
                Coordinator.Instance.UserManager.PerformLogout();
                NavigateToView(typeof(Login));
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

        private void NavigateToView(Type viewType)
        {
            PatternzView? nextView = Activator.CreateInstance(viewType) as PatternzView;
            if (nextView != null)
            {
                CurrentPage = nextView;
                if (CurrentPage.ViewModel != null)
                {
                    CurrentPage.ViewModel.Navigate = NavigateToView;
                }
                PrepareMenuItems();
            }
            else
            {
                ThrowHelper.ThrowInvalidOperationException($"Unable to navigate to page, unrecognizable type: {viewType}");
            }
        }

        private void QuitApplication(object? _)
        {
            Action exit = () => Application.Current.Shutdown();
            if (CurrentPage.ViewModel != null)
            {
                CurrentPage.ViewModel.RequestExit(exit);
            }
            else 
            {
                exit(); 
            }
        }

        private void About(object? _)
        {
            ShowPopupMessage aboutMsg = new("About Patternz", "Patternz is an educational application " +
                "designed to explore and apply various design patterns to create an extensible, robust " +
                "application framework.");
            Messenger.Send(aboutMsg);
        }

        private void PrepareMenuItems()
        {
            FileMenuItems = CurrentPage.ViewModel?.FileMenuItems ?? new ObservableCollection<MenuItem>();
            FileMenuItems.Add(new MenuItem() { Header = "Exit", Command = ExitCommand });
            AttachMenuItemNames(FileMenuItems);
            NotifyPropertyChanged(nameof(FileMenuItems));

            EditMenuItems = CurrentPage.ViewModel?.EditMenuItems ?? new ObservableCollection<MenuItem>();
            AttachMenuItemNames(EditMenuItems);
            NotifyPropertyChanged(nameof(EditMenuItems));

            ViewMenuItems = CurrentPage.ViewModel?.ViewMenuItems ?? new ObservableCollection<MenuItem>();
            AttachMenuItemNames(ViewMenuItems);
            NotifyPropertyChanged(nameof(ViewMenuItems));

            HelpMenuItems = CurrentPage.ViewModel?.HelpMenuItems ?? new ObservableCollection<MenuItem>();
            HelpMenuItems.Add(new MenuItem() { Header = "About Application", Command = AboutCommand });
            AttachMenuItemNames(HelpMenuItems);
            NotifyPropertyChanged(nameof(HelpMenuItems));

            NotifyPropertyChanged(nameof(IsMenuVisible));
            
        }

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
