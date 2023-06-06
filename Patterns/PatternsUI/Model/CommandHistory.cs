using Patterns.Command;
using System.Collections.Generic;

namespace PatternsUI.Model
{
    internal class CommandHistory
    {
        private const int EmptyCommandHistoryIndex = -1;

        public int CurrentIndex { get; private set; } = EmptyCommandHistoryIndex;
       
        public IPatternzCommand? LastExecutedCommand { get; private set; }

        public string FileName { get; set; } = string.Empty;
        public List<IPatternzCommand> History { get; set; } = new List<IPatternzCommand>();

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
            IPatternzCommand command = History[CurrentIndex];
            command.Execute();
            LastExecutedCommand = command;
        }

        public void Undo()
        {
            IPatternzCommand command = History[CurrentIndex];
            command.Unexecute();            
            CurrentIndex -= 1;
            LastExecutedCommand = command;
        }

        /// <summary>
        /// Adds a command to the history. If the CurrentIndex is less than the 
        /// history count (such as if the Undo operation was executed), this will replace
        /// the existing command at that index.
        /// </summary>
        /// <param name="command">The command to add to history</param>
        /// <param name="wasExecuted">Optional. Flag indicating this command was the last command executed</param>
        public void AddCommand(IPatternzCommand command, bool wasExecuted = true)
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

        public List<IPatternzCommand> GetRelativeHistory(int fromIndex)
        {
            if (History.Count == 0 || fromIndex == CurrentIndex)
            {
                return new List<IPatternzCommand>();
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
