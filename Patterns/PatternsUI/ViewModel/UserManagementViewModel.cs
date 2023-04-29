using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
using PatternsUI.View;
using System.Windows.Controls;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// Manages users, allowing adding, updating, and removing from the store
    /// </summary>
    public class UserManagementViewModel : ViewModelBase
    {
        public RelayCommand NavigateToDataRecordsCommand { get; private set; }
        public RelayCommand ShowAboutCommand { get; private set; }
        public UserManagementViewModel() 
        {
            NavigateToDataRecordsCommand = new RelayCommand(NavigateToDataRecords);
            ShowAboutCommand = new RelayCommand(ShowAbout);

            PrepareMenuItems();
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
                new MenuItem() { Header = "About User Management", Command = ShowAboutCommand }
            };
        }
    }
}
