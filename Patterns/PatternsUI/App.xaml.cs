using Patterns;
using System.Windows;

namespace PatternsUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Coordinator.Instance.Configure(PatternsUI.Properties.Settings.Default);
        }
    }
}
