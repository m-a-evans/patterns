using System;

namespace PatternsUI.MVVM
{
    /// <summary>
    /// Interface for view models used in Patternz
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Method for performing actions when the viewmodel has been loaded
        /// </summary>
        void OnLoaded();

        /// <summary>
        /// Method for performing actions when the viewmodel has been unloaded
        /// </summary>
        void OnUnloaded();

        /// <summary>
        /// Method to allow the viewmodel to intercept an action that would unload the viewmodel
        /// </summary>
        /// <param name="exit">The exit action to perform, if this method allows it</param>
        /// <param name="message">A context message about the exit</param>
        void RequestExit(Action exit, string? message = null);

        /// <summary>
        /// Allows passing a context to the view model
        /// </summary>
        /// <param name="context"></param>
        void ApplyContext(object? context = null);
    }
}
