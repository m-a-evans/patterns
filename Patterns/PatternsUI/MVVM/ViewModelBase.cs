using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace PatternsUI.MVVM
{
    /// <summary>
    /// Provides a base class for the various view models
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public ObservableCollection<MenuItem> FileMenuItems { get; protected set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> EditMenuItems { get; protected set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> HelpMenuItems { get; protected set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> ViewMenuItems { get; protected set; } = new ObservableCollection<MenuItem>();

        /// <summary>
        /// Method for handling the case when the view is being exited, in case there are
        /// last minute decisions to be made
        /// </summary>
        /// <param name="exit">The exit action to perform</param>
        /// <param name="message">The context message for this exit</param>
        public virtual void RequestExit(Action exit, string? message = null)
        {
            exit();
        }

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
        protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
