using CommunityToolkit.Diagnostics;
using Patterns;
using Patterns.Account;
using PatternsUI.MVVM;
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
    internal class MainWindowViewModel : ViewModelBase
    {
        private PatternzView _currentView;
        public PatternzView CurrentPage { 
            get => _currentView; 
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    NotifyPropertyChanged(nameof(CurrentPage));
                }
            }
        }
        public RelayCommand LogoutCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }

        public bool IsMenuVisible { get => CurrentPage.GetType() != typeof(Login); }
        public ImageSource CurrentUserPicture { get; private set; }

        public MainWindowViewModel()
        {
            LogoutCommand = new RelayCommand(Logout, (_) => 
                GlobalStateSingleton.Instance.CurrentUser != PatternzUser.AnyUser);
            ExitCommand = new RelayCommand(QuitApplication);

            GlobalStateSingleton.Instance.CurrentUserChanged += OnCurrentUserChanged;

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
            NotifyPropertyChanged(nameof(CurrentUserPicture));
        }

        public void Logout(object? _)
        {
            Action logout = () =>
            {
                GlobalStateSingleton.Instance.PerformLogout();
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

        private void PrepareMenuItems()
        {
            FileMenuItems = CurrentPage.ViewModel?.FileMenuItems ?? new ObservableCollection<MenuItem>();
            FileMenuItems.Add(new MenuItem() { Header = "Exit", Command = ExitCommand });
            EditMenuItems = CurrentPage.ViewModel?.EditMenuItems ?? new ObservableCollection<MenuItem>();
            ViewMenuItems = CurrentPage.ViewModel?.ViewMenuItems ?? new ObservableCollection<MenuItem>();
            HelpMenuItems = CurrentPage.ViewModel?.HelpMenuItems ?? new ObservableCollection<MenuItem>();

            NotifyPropertyChanged(nameof(FileMenuItems));
            NotifyPropertyChanged(nameof(EditMenuItems));
            NotifyPropertyChanged(nameof(HelpMenuItems));
            NotifyPropertyChanged(nameof(IsMenuVisible));
            NotifyPropertyChanged(nameof(ViewMenuItems));
        }
    }
}
