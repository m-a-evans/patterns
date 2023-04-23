using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsUI.MVVM
{
    /// <summary>
    /// Provides a base class for the various view models
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The action which will execute to navigate away from the page associated with this view model
        /// </summary>
        public Action<Type> Navigate { get; set; } = (Type _) => throw new NotImplementedException("Navigate command was not defined for this view model");

        /// <summary>
        /// Event handler for when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Method to alert property changed handlers that a property has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
