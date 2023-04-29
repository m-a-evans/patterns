using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
using System;
using System.Windows;

namespace PatternsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RegisterForPopups();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Messenger.UnregisterAll(this);
        }

        private void RegisterForPopups()
        {
            Messenger.Register<ShowPopupMessage>(this, ShowPopup);
            Messenger.Register<ShowYesNoPopupMessage>(this, ShowYesNoPopup);
        }

        private void ShowPopup(IMessage message)
        {
            ShowPopupMessage msg = (ShowPopupMessage)message;

            Shade.Visibility = Visibility.Visible;
            MessageBox.Show(msg.Contents, msg.Title);
            Shade.Visibility = Visibility.Collapsed;
        }

        private void ShowYesNoPopup(IMessage message) 
        {
            ShowYesNoPopupMessage msg = (ShowYesNoPopupMessage)message;

            Shade.Visibility = Visibility.Visible;
            MessageBoxResult result = MessageBox.Show(msg.Contents, msg.Title, MessageBoxButton.YesNo);
            Shade.Visibility = Visibility.Collapsed;

            msg.OnUserSelection(result == MessageBoxResult.Yes);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            LogoutButton.Visibility = Visibility.Visible;
        }

        private void ContentViewGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LogoutButton.Visibility = Visibility.Collapsed;
        }
    }
}
