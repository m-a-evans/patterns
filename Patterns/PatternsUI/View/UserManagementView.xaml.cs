using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
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
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnRetrievePassword(IMessage message)
        {
            if (message is RetrievePasswordMessage msg)
            {
                msg.Callback(PasswordBox.Password);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Messenger.Register<ClearUIMessage>(this, OnClearUIMessage);
            Messenger.Register<RetrievePasswordMessage>(this, OnRetrievePassword);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Messenger.UnregisterAll(this);
        }

        private void OnClearUIMessage(IMessage message)
        {
            PasswordBox.Password = string.Empty;
        }

        private void UsernameTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible && isVisible)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    UsernameTextBox.Focus();
                });
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Visibility == Visibility.Visible)
            {
                UsernameTextBox.Focus();
            }
        }
    }
}
