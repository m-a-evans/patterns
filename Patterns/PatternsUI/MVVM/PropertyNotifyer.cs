using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PatternsUI.MVVM
{
    public class PropertyNotifyer : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <summary>
        /// Method to alert property changed handlers that a property has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Method to alert property changed handlers that a property is changing
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}
