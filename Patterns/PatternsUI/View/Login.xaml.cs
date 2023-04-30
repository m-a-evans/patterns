using PatternsUI.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace PatternsUI.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : PatternzView
    {
        public Login()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            UserName.Focus();
        }

        /// <summary>
        /// Checks to see whether the submit button can be enabled. Also
        /// links the "enter" button to the submit command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            SubmitButton.IsEnabled = RequiredFieldsFilled();
            if (e.Key == Key.Enter && SubmitButton.IsEnabled)
            {
                Submit();
            }
        }

        /// <summary>
        /// Executes the submit command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            Submit();
        }

        /// <summary>
        /// Executes the submit command on the viewmodel
        /// </summary>
        private void Submit()
        {
            if (ViewModel is LoginViewModel viewModel) 
            {
                viewModel.SubmitCommand.Execute(Password.Password);
            }
        }

        /// <summary>
        /// Returns true if every necessary field has a value
        /// </summary>
        /// <returns></returns>
        private bool RequiredFieldsFilled()
        {
            return Password.Password.Length > 0 && UserName.Text.Length > 0;
        }
    }
}
