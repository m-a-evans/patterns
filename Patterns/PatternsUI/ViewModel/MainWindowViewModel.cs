using CommunityToolkit.Diagnostics;
using Patterns;
using Patterns.Account;
using PatternsUI.MVVM;
using PatternsUI.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PatternsUI.ViewModel
{
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
                    NotifyPropertyChanged(nameof(FileMenuItems));
                    NotifyPropertyChanged(nameof(EditMenuItems));
                    NotifyPropertyChanged(nameof(HelpMenuItems));
                    NotifyPropertyChanged(nameof(IsMenuVisible));
                }
            }
        }

        public RelayCommand LogoutCommand { get; private set; }

        public RelayCommand ExitCommand { get; private set; }

        public ObservableCollection<MenuItem> FileMenuItems { get; private set; }
        public ObservableCollection<MenuItem> EditMenuItems { get; private set; }
        public ObservableCollection<MenuItem> HelpMenuItems { get; private set; }

        public bool IsMenuVisible { get => CurrentPage.GetType() != typeof(Login); }

        public string CurrentUserPicUrl { get => GlobalStateSingleton.Instance.CurrentUser.PictureUrl ?? string.Empty; }

        public MainWindowViewModel()
        {
            LogoutCommand = new RelayCommand(Logout, (_) => GlobalStateSingleton.Instance.CurrentUser != PatternzUser.AnyUser);
            ExitCommand = new RelayCommand(Exit);

            NavigateToView(typeof(Login));
        }

        public void Logout(object? _)
        {
            GlobalStateSingleton.Instance.PerformLogout();
            NavigateToView(typeof(Login));
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

        private void Exit(object? _)
        {
            Application.Current.Shutdown();
        }

        private void PrepareMenuItems()
        {
            FileMenuItems = CurrentPage.FileMenuItems;
            FileMenuItems.Add(new MenuItem() { Header = "Exit", Command = ExitCommand });
            EditMenuItems = CurrentPage.EditMenuItems;
            HelpMenuItems = CurrentPage.HelpMenuItems;
        }
    }
}
