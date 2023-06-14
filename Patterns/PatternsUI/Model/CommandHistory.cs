using CommunityToolkit.Diagnostics;
using Patterns.Command;
using Patterns.Data.Command;
using System;
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
                // If we are replacing a command, that means one or more undo operations
                // occurred, and we are starting a new history from that point on
                History[CurrentIndex] = command;
                RemoveHistoryEntriesAfterIndex(CurrentIndex);
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

        /// <summary>
        /// Returns a range of commands based on the index. If the current index
        /// is before
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <returns></returns>
        public List<DataCommand> GetRelativeHistory(int fromIndex)
        {
            if (History.Count == 0)
            {
                return new List<DataCommand>();
            }
            else if (fromIndex <= CurrentIndex)
            {
                // Get everything from the fromIndex up to the current index
                int minIndex = Math.Max(0, fromIndex);
                return History.GetRange(minIndex, CurrentIndex + 1 - minIndex);
            }
            else
            {
                // Get everything from the current index up to the fromIndex
                int maxIndex = Math.Min(History.Count - 1, fromIndex);
                return History.GetRange(CurrentIndex, maxIndex - CurrentIndex + 1);
            }
        }

        /// <summary>
        /// Removes all History entries after the supplied index. Leaves the element at the index intact.
        /// </summary>
        /// <param name="index"></param>
        private void RemoveHistoryEntriesAfterIndex(int index) 
        {
            if (index >= History.Count || index < 0)
            {
                ThrowHelper.ThrowArgumentException($"Index out of range. given: {index} valid range: [0,{History.Count})");
            }
            for (int i = History.Count - 1; i > index; i--)
            {
                History.RemoveAt(i);
            }
        }
    }
}
