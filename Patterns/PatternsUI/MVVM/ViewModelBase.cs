using CommunityToolkit.Diagnostics;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PatternsUI.MVVM
{
    /// <summary>
    /// Provides a base class for the various view models
    /// </summary>
    public abstract class ViewModelBase : PropertyNotifyer, IViewModel, INavigator
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

        public virtual void OnLoaded()
        {
            // no op
        }

        public virtual void OnUnloaded()
        {
            // no op
        }

        public Action<Type, object?> NavAction { get; set; } = (type, _) => ThrowHelper.ThrowInvalidOperationException("Navigation action not defined for this view model");
        public virtual void Navigate<T>(object? context = null) 
        {
            NavAction(typeof(T), context);
        }

        public virtual void ApplyContext(object? context = null)
        {
            // no op
        }
    }
}
