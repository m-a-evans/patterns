using Patterns.Command;
using Patterns.Data.Command;
using System.Collections.Generic;

namespace PatternsUI.Model
{
    internal class CommandHistory
    {
        private const int EmptyCommandHistoryIndex = -1;

        public int CurrentIndex { get; private set; } = EmptyCommandHistoryIndex;
       
        public DataCommand? LastExecutedCommand { get; private set; }

        public string FileName { get; set; } = string.Empty;
        public List<DataCommand> History { get; set; } = new List<DataCommand>();

        public bool CanRedo()
        {
            return CurrentIndex < History.Count - 1;
        }

        public bool CanUndo() 
        {
            return CurrentIndex > EmptyCommandHistoryIndex;
        }

        public void Redo()
        {
            CurrentIndex += 1;
            DataCommand command = History[CurrentIndex];
            LastExecutedCommand = command;
            command.Execute();            
        }

        public void Undo()
        {
            DataCommand command = History[CurrentIndex];
            LastExecutedCommand = command;
            command.Unexecute();            
            CurrentIndex -= 1;            
        }

        /// <summary>
        /// Adds a command to the history. If the CurrentIndex is less than the 
        /// history count (such as if the Undo operation was executed), this will replace
        /// the existing command at that index.
        /// </summary>
        /// <param name="command">The command to add to history</param>
        /// <param name="wasExecuted">Optional. Flag indicating this command was the last command executed</param>
        public void AddCommand(DataCommand command, bool wasExecuted = true)
        {
            CurrentIndex += 1;

            if (wasExecuted)
            {
                command.State = CommandState.Executed;
                LastExecutedCommand = command;
            }

            if (History.Count == CurrentIndex)
            {
                History.Add(command);
            }
            else
            {
                History[CurrentIndex] = command;
            }            
        }

        public void Reset()
        {
            FileName = string.Empty;
            CurrentIndex = EmptyCommandHistoryIndex;
            History.Clear();
        }

        public void ExecuteEntireHistory()
        {
            CurrentIndex = EmptyCommandHistoryIndex;
            // Redo all commands stored in the history 
            while (CanRedo())
            {
                Redo();
            }
        }

        public List<DataCommand> GetRelativeHistory(int fromIndex)
        {
            if (History.Count == 0 || fromIndex == CurrentIndex)
            {
                return new List<DataCommand>();
            }
            else if (fromIndex < CurrentIndex)
            {
                return History.GetRange(fromIndex, CurrentIndex + 1);
            }
            else
            {
                return History.GetRange(CurrentIndex + 1, fromIndex);
            }
        }
    }
}
