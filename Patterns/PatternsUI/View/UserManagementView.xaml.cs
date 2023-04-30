using PatternsUI.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PatternsUI.View
{
    /// <summary>
    /// Interaction logic for UserManagementView.xaml
    /// </summary>
    public partial class UserManagementView : PatternzView
    {
        public UserManagementView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Executes the save command on the view model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is UserManagementViewModel viewModel)
            {
                if (PasswordBox.Password.Length > 0)
                {
                    viewModel.SaveCommand.Execute(PasswordBox.Password);
                }
                else
                {
                    viewModel.SaveCommand.Execute(null);
                }
            }
        }
    }
}
