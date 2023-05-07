using System;

namespace PatternsUI.MVVM
{
    /// <summary>
    /// Interface for objects that can navigate around the application
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// The navigation action
        /// </summary>
        Action<Type, object?> NavAction { get; set; }

        /// <summary>
        /// Invoke the navigation
        /// </summary>
        /// <typeparam name="T">The Type of view to navigate to</typeparam>
        /// <param name="context">Optional. Context for the new view</param>
        void Navigate<T>(object? context = null);
    }
}
