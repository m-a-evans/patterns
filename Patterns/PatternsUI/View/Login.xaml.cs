using PatternsUI.View;
using PatternsUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        public void OnPasswordChanged(object sender,  RoutedEventArgs e)
        {
            if (ViewModel is LoginViewModel viewModel && sender is PasswordBox pwBox)
            {
                viewModel.Password = pwBox.Password;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SubmitButton.IsEnabled && ViewModel is LoginViewModel viewModel)
            {
                viewModel.SubmitCommand.Execute(null);
            }
        }
    }
}
