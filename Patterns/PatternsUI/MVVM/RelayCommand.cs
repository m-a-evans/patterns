using CommunityToolkit.Diagnostics;
using System;
using System.Windows.Input;

namespace PatternsUI.MVVM
{
    /// <summary>
    /// Simple implementation of ICommand
    /// </summary>
    internal class RelayCommand : ICommand
    {
        /// <summary>
        /// The action to execute when the command is invoked
        /// </summary>
        private readonly Action<object?> _execute;
        /// <summary>
        /// The conditional which determines whether the command is allowed to execute or not
        /// </summary>
        private readonly Predicate<object?> _canExecute;

        /// <summary>
        /// RelayCommand that is always able to execute
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<object?> execute) : this(execute, (obj) => true)
        {
        }

        /// <summary>
        /// RelayCommand that is only able to execute if the provided Predicate returns true
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            if (execute == null)
                ThrowHelper.ThrowArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Checks if this command can be executed based on the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Allows the CommandManager to know whether or not to check commands to determine if and what can be executed,
        /// updating the UI accordingly
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Executes the RelayCommand with the provided parameter
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
