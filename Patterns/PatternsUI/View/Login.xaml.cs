using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
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
            Unloaded += OnUnloaded;
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            UserName.Focus();
            Messenger.Register<RetrievePasswordMessage>(this, OnRetrievePasswordMessage);
        }

        public void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Messenger.UnregisterAll(this);
        }

        /// <summary>
        /// Gets the password out of the password box when something asks for it.
        /// Because the password box protects the entered value, it cannot be
        /// bound normally, and must be explicitly extracted
        /// </summary>
        /// <param name="message"></param>
        private void OnRetrievePasswordMessage(IMessage message)
        {
            if (message is RetrievePasswordMessage retrievePw)
            {
                retrievePw.Callback(Password.Password);
            }
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
