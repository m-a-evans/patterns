using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
using PatternsUI.View;
using System;
using System.Windows.Controls;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// Handles the actual data record management of the application, such as loading, saving, editing and deleting records
    /// </summary>
    public class DataRecordsViewModel : ViewModelBase
    {
        #region Fields

        #endregion

        #region Properties

        public bool IsDirty = false;
        public RelayCommand NavigateToUserManagementCommand { get; private set; }
        public RelayCommand ShowAboutCommand { get; private set; }

        #endregion

        #region Constructors and Methods
        public DataRecordsViewModel() 
        {
            ShowAboutCommand = new RelayCommand(ShowAbout);
            NavigateToUserManagementCommand = new RelayCommand(NavigateToUserManagement);

            PrepareMenuItems();
        }

        public override void RequestExit(Action exit, string? message)
        {
            if (IsDirty) 
            {
                message ??= "Are you sure you want to exit?\nUnsaved changes will be lost.";
                ShowYesNoPopupMessage ConfirmExitMessage = new("Leaving Data Records", message!, (confirmed) =>
                {
                    if (confirmed) exit();
                });
                Messenger.Send(ConfirmExitMessage);
            }
            else
            {
                exit();
            }
        }

        #endregion

        #region Private Methods

        private void NavigateToUserManagement(object? _)
        {
            Action navAction = () => Navigate(typeof(UserManagementView));
            if (IsDirty) 
            {
                RequestExit(navAction, "Are you sure you want to leave?");
            }
            else
            {
                navAction();
            }
        }

        private void PrepareMenuItems()
        {
            HelpMenuItems = new()
            {
                new MenuItem() { Header = "About This Page", Command = ShowAboutCommand }
            };
            ViewMenuItems = new()
            {
                new MenuItem() { Header = "Users", Command = NavigateToUserManagementCommand }
            };
        }

        private void ShowAbout(object? _)
        {
            Messenger.Send(new ShowPopupMessage("About Data Records", "The Data Records page is where you can add, edit and remove data records, as well as load and save them." +
                "\n\nThis page makes use of the command pattern to allow you to undo or redo, as well as restore interrupted work." +
                "\n\nThis page also makes use of the factory pattern to allow you to easily switch between save formats." +
                "\n\nFinally, this page makes use of the proxy pattern, ensuring the current user has adequate rights to perform these actions."));
        }

        #endregion
    }
}
