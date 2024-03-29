﻿using CommunityToolkit.Diagnostics;
using Microsoft.Win32;
using Patterns;
using Patterns.Data.Command;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using Patterns.IO;
using PatternsUI.Model;
using PatternsUI.MVVM;
using PatternsUI.MVVM.Messages;
using PatternsUI.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// Handles the actual data record management of the application, such as loading, saving, editing and deleting records
    /// </summary>
    public class DataRecordsViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Some properties are "automated" and so should be ignored when it comes
        /// to handling data record updates
        /// </summary>
        private readonly List<string> _ignorableDataRecordProperties = new()
        {
            nameof(DataRecord.Empty.Id),
            nameof(DataRecord.Empty.CreatedDate),
            nameof(DataRecord.Empty.DateModified)
        };

        private CommandHistory _commandHistory;
        private int _commandHistorySaveIndex;

        private DataFile? _currentFile;
        private DataRecord? _oldRecord;

        private readonly object _saveHistoryLock = new();

        private const string DefaultDirectory = "../../../../data";

        private ObservableCollection<DataRecord> _dataRecords = new();

        #endregion

        #region Properties

        public ObservableCollection<DataRecord> DataRecords 
        {
            get
            {
                return _dataRecords;
            }
            set
            {
                if (_dataRecords != value)
                {
                    _dataRecords = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DataFile? DataFile { get { return _currentFile; } }

        public bool IsDirty => _commandHistory.CurrentIndex != _commandHistorySaveIndex;

        public bool IsFileXml 
        { 
            get
            {
                return _currentFile?.Format == DataRecordFormat.Xml;
            }
        }
        public bool IsFileLoaded { get => _currentFile != null; }
        public bool IsFileNew { get; set; } = false;
        public bool IsEditingFileName { get; set; } = false;
        public string NewFileName { get; set; } = string.Empty;
        public string FileName { get => GetDisplayFileName(_currentFile?.FileName); }

        public RelayCommand EditFileNameCommand { get; private set; }
        public RelayCommand XmlSelectedCommand { get; private set; }
        public RelayCommand JsonSelectedCommand { get; private set; }
        public RelayCommand NavigateToUserManagementCommand { get; private set; }
        public RelayCommand ShowAboutCommand { get; private set; }
        public RelayCommand NewFileCommand { get; private set; }
        public RelayCommand OpenFileCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand UndoCommand { get; private set; }
        public RelayCommand RedoCommand { get; private set; }
        public RelayCommand SaveFileNameCommand { get; private set; }


        #endregion

        #region Constructors and Methods
        public DataRecordsViewModel() 
        {
            _commandHistory = new();
            _commandHistorySaveIndex = _commandHistory.CurrentIndex;

            ShowAboutCommand = new RelayCommand(ShowAbout);
            NavigateToUserManagementCommand = new RelayCommand(NavigateToUserManagement);
            NewFileCommand = new RelayCommand(NewFile);
            OpenFileCommand = new RelayCommand(OpenFile);
            SaveCommand = new RelayCommand(SaveData, CanSaveCommandExecute);
            CloseCommand = new RelayCommand(CloseCurrentFile, CanCloseCommandExecute);
            UndoCommand = new RelayCommand(UndoLastCommand, CanUndoCommandExecute);
            RedoCommand = new RelayCommand(RedoLastCommand, CanRedoCommandExecute);
            EditFileNameCommand = new RelayCommand(SetEditFileNameMode);
            XmlSelectedCommand = new RelayCommand(SelectXmlFormat);
            JsonSelectedCommand = new RelayCommand(SelectJsonFormat);
            SaveFileNameCommand = new RelayCommand(CreateNewDataFile);

            PrepareMenuItems();
        }

        public override void RequestExit(Action exit, string? message)
        {
            if (IsDirty) 
            {
                message ??= "Are you sure you want to exit?\nUnsaved changes will be lost.";
                ShowYesNoPopupMessage ConfirmExitMessage = new("Leaving Data Records", message!, (confirmed) =>
                {
                    if (confirmed)
                    {
                        DisableDataRecordEventListeners();
                        exit();
                    }                        
                });
                Messenger.Send(ConfirmExitMessage);
            }
            else
            {
                DisableDataRecordEventListeners();
                exit();
            }
        }

        #endregion

        #region Private Methods
        
        private string GetDisplayFileName(string? fullFileName)
        {
            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                return string.Empty;
            }
            return Path.GetFileNameWithoutExtension(fullFileName);
        }

        /// <summary>
        /// Starts event listeners for data record change events
        /// </summary>
        private void EnableDataRecordEventListeners()
        {
            _dataRecords.CollectionChanged += OnDataRecordCollectionChanged;
            AddAllDataRecordListeners();
        }

        /// <summary>
        /// Stops event listeners for data record change events
        /// </summary>
        private void DisableDataRecordEventListeners()
        {
            _dataRecords.CollectionChanged -= OnDataRecordCollectionChanged;
            RemoveAllDataRecordListeners();
        }

        /// <summary>
        /// Adds event listeners to every record in the current data set
        /// </summary>
        private void AddAllDataRecordListeners()
        {
            foreach (DataRecord record in _dataRecords)
            {
                record.PropertyChanged += OnDataRecordChanged;
                record.PropertyChanging += OnDataRecordChanging;
            }
        }

        /// <summary>
        /// Removes all event listeners from every record in the current data set
        /// </summary>
        private void RemoveAllDataRecordListeners()
        {
            foreach (DataRecord record in _dataRecords) 
            {
                record.PropertyChanged -= OnDataRecordChanged;
                record.PropertyChanging -= OnDataRecordChanging;
            }
        }

        /// <summary>
        /// Event handler for when data records are added or removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataRecordCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?.Count > 0 && e.NewItems[0] is DataRecord record) 
                {
                    record.PropertyChanging += OnDataRecordChanging;
                    record.PropertyChanged += OnDataRecordChanged;                    
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems?.Count > 0 && e.OldItems[0] is DataRecord record)
                {
                    PushToCommandHistory(new RemoveDataRecordCommand(_dataRecords, new RemoveDataRecordParam(record)));
                }
            }
        }

        /// <summary>
        /// Event handler for when a data record is in the process of changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataRecordChanging(object? sender, PropertyChangingEventArgs e)
        {
            if (sender is DataRecord record)
            {                
                // If the property is one we can ignore, skip processing
                if (_ignorableDataRecordProperties.Find(x => x == e.PropertyName) != null)
                {
                    return;
                }
                _oldRecord = record.DeepCopy();
            }
        }

        /// <summary>
        /// Event handler for when a data record has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataRecordChanged(object? sender, PropertyChangedEventArgs e) 
        {
            if (sender is DataRecord record)
            {
                record.PropertyChanged -= OnDataRecordChanged;
                record.PropertyChanging -= OnDataRecordChanging;
                // If the property is one we can ignore, skip processing
                if (_ignorableDataRecordProperties.Find(x => x == e.PropertyName) != null)
                {
                    return;
                }

                record.DateModified = DateTime.UtcNow;

                if (record.Id == Guid.Empty) 
                {
                    record.Id = Guid.NewGuid();
                    record.CreatedDate = DateTime.UtcNow;
                    PushToCommandHistory(new CreateDataRecordCommand(_dataRecords, new CreateDataRecordParam(record)));
                }
                else 
                {
                    PushToCommandHistory(new EditDataRecordCommand(_dataRecords, new EditDataRecordParam(record, _oldRecord)));
                }
                record.PropertyChanged += OnDataRecordChanged;
                record.PropertyChanging += OnDataRecordChanging;
            }
        }

        /// <summary>
        /// Prepares a new DataFile to be the current file
        /// </summary>
        /// <param name="name"></param>
        private void CreateNewDataFile(object? name)
        {
            string fileName = name as string ?? NewFileName;
            if (_currentFile == null)
            {
                _currentFile = new DataFile();
            }
            else
            {
                DisableDataRecordEventListeners();
            }
            _currentFile.Format = IsFileXml ? DataRecordFormat.Xml : DataRecordFormat.Json;            
            _currentFile.Path = DefaultDirectory;
            _currentFile.FileName = _currentFile.Path + "/" + AppendFileExtensionIfAbsent(fileName, _currentFile.Format);
            
            EnableDataRecordEventListeners();
            SaveData(null);         
        }

        /// <summary>
        /// Adds a file extension to a file name based on its format. If the extension already exists on the file,
        /// this method does nothing
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private string AppendFileExtensionIfAbsent(string fileName, DataRecordFormat format)
        {
            if (!Path.HasExtension(fileName))
            {
                switch (format) 
                {
                    case DataRecordFormat.Xml:
                        return fileName + ".xml";
                    case DataRecordFormat.Json:
                    default:
                        return fileName + ".json";
                }
            }
            return fileName;
        }

        /// <summary>
        /// Checks if the current state allows for closing the current file
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        private bool CanCloseCommandExecute(object? _)
        {
            return IsFileLoaded || IsEditingFileName;
        }

        /// <summary>
        /// Checks if the user is allowed to redo the last executed command
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        private bool CanRedoCommandExecute(object? _)
        {
            return _commandHistory.CanRedo();            
        }

        /// <summary>
        /// Executes the undone command that was after the most recently used command.
        /// </summary>
        /// <param name="_"></param>
        private void RedoLastCommand(object? _)
        {
            DisableDataRecordEventListeners();
            _commandHistory.Redo();
            EnableDataRecordEventListeners();
            NotifyAllProperties();
        }

        /// <summary>
        /// Checks if the user is allowed to undo the last executed command
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        private bool CanUndoCommandExecute(object? _)
        {
            return _commandHistory.CanUndo();
        }

        /// <summary>
        /// Undoes the last executed command
        /// </summary>
        /// <param name="_"></param>
        private void UndoLastCommand(object? _)
        {
            DisableDataRecordEventListeners();
            _commandHistory.Undo();
            EnableDataRecordEventListeners();
            NotifyAllProperties();
        }

        /// <summary>
        /// Adds a DataCommand to the command history
        /// </summary>
        /// <param name="cmd"></param>
        private void PushToCommandHistory(DataCommand cmd)
        {
            _commandHistory.AddCommand(cmd);
        }

        /// <summary>
        /// Prepares the state to allow for editing the current file name
        /// </summary>
        /// <param name="_"></param>
        private void SetEditFileNameMode(object? _)
        {
            IsEditingFileName = true;
            NewFileName = FileName;
            NotifyAllProperties();
        }

        /// <summary>
        /// Updates the current file format to be JSON
        /// </summary>
        /// <param name="_"></param>
        private void SelectJsonFormat(object? _)
        {
            if (IsFileXml)
            {
                SetDataRecordFormatCommand cmd = new(DataFile, new SetDataRecordFormatParam(DataRecordFormat.Json));
                cmd.Execute();
                PushToCommandHistory(cmd);
            }
        }

        /// <summary>
        /// Updates the current file format to be XML
        /// </summary>
        /// <param name="_"></param>
        private void SelectXmlFormat(object? _)
        {
            if (!IsFileXml)
            {
                SetDataRecordFormatCommand cmd = new(DataFile, new SetDataRecordFormatParam(DataRecordFormat.Xml));
                cmd.Execute();
                PushToCommandHistory(cmd);
            }
        }

        /// <summary>
        /// Returns true if the user is allowed to save a file given the current state
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        private bool CanSaveCommandExecute(object? _)
        {
            return IsDirty && IsFileLoaded;
        }

        /// <summary>
        /// Persists the current file's contents to storage. This will set the save index to the current 
        /// command history index
        /// </summary>
        /// <param name="_"></param>
        private void SaveData(object? _)
        {
            if (_currentFile == null)
            {
                ThrowHelper.ThrowInvalidOperationException("Unable to save data. Current File is null");
            }


            List<DataRecord> temp = new();
            // Make sure the file we're saving has the appropriate extension based on format
            _currentFile.FileName = AppendFileExtensionIfAbsent(Path.GetFileNameWithoutExtension(_currentFile.FileName), _currentFile.Format);
            try
            {
                IDataRecordManager dataRecordManager =
                Coordinator.Instance.GetDataRecordManager(IsFileXml ? DataRecordFormat.Xml : DataRecordFormat.Json);
                CopyRecordsFromFile(_currentFile, temp);
                CopyRecordsToFile(_currentFile, DataRecords);
                _ = dataRecordManager.WriteDataRecords(_currentFile);
            }
            catch (UnauthorizedAccessException)
            {
                CopyRecordsToFile(_currentFile, temp);
                Messenger.Send(new ShowPopupMessage("Access Denied", "You do not have the required permissions to perform this operation"));
                return;
            }
            _commandHistorySaveIndex = _commandHistory.CurrentIndex;

            IsEditingFileName = false;
            NotifyAllProperties();
        }

        /// <summary>
        /// Reads the data records from a record collection into a DataFile
        /// </summary>
        /// <param name="file">The file to read the records into</param>
        /// <param name="records">The records to read</param>
        private void CopyRecordsToFile(DataFile file, ICollection<DataRecord> records)
        {
            file.DataRecords.Clear();
            foreach (DataRecord record in records) 
            {                
                if (record != DataRecord.Empty)
                {
                    file.DataRecords.Add(record.CreatedDate, record);
                }
            }
        }

        /// <summary>
        /// Reads the data records from a file into the record collection
        /// </summary>
        /// <param name="file">The file to read records from</param>
        /// <param name="records">The collection to add the read records into</param>
        private void CopyRecordsFromFile(DataFile file, ICollection<DataRecord> records)
        {
            records.Clear();
            foreach (DataRecord record in file.DataRecords.Values)
            {                
                records.Add(record);
            }
        }

        /// <summary>
        /// Prepares the state for adding a new file
        /// </summary>
        /// <param name="_"></param>
        private void NewFile(object? _)
        {
            ResetAllProperties();
            IsEditingFileName = true;
            IsFileNew = true;
            NotifyPropertyChanged(nameof(IsEditingFileName));
            NotifyPropertyChanged(nameof(IsFileNew));
        }

        /// <summary>
        /// Opens a dialog to find and load a DataFile 
        /// </summary>
        /// <param name="_"></param>
        private void OpenFile(object? _)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = "xml,json";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() ?? false)
            {
                LoadFileByName(dialog.FileName);
            }
        }

        /// <summary>
        /// Loads a file by name, setting the current file and its contents in memory. 
        /// The filename must be the fully qualified path of the file.
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadFileByName(string fileName)
        {
            _currentFile = null;
            IDataRecordManager dataRecordManager = Coordinator.Instance.GetDataRecordManager(fileName);
            try
            {
                _ = dataRecordManager.TryParseRecords(fileName, out _currentFile);
            }
            catch (UnauthorizedAccessException)
            {
                Messenger.Send(new ShowPopupMessage("Access Denied", "You do not have the require permission to perform this operation"));
                return;
            }

            if (_currentFile != null)
            {
                IsFileNew = false;
                IsEditingFileName = false;
                CopyRecordsFromFile(_currentFile, _dataRecords);

                EnableDataRecordEventListeners();
                NotifyAllProperties();
            }
        }

        /// <summary>
        /// Resets all properties to their default values, and clears command history
        /// </summary>
        private void ResetAllProperties()
        {
            IsEditingFileName = false;
            IsFileNew = false;

            _currentFile = null;
            _commandHistory.Reset();
            _commandHistorySaveIndex = _commandHistory.CurrentIndex;
            _dataRecords.Clear();

            NotifyAllProperties();
        }

        /// <summary>
        /// Calls NotifyPropertyChanged on all properties
        /// </summary>
        private void NotifyAllProperties()
        {
            NotifyPropertyChanged(nameof(IsDirty));
            NotifyPropertyChanged(nameof(IsEditingFileName));
            NotifyPropertyChanged(nameof(IsFileLoaded));
            NotifyPropertyChanged(nameof(IsFileNew));
            NotifyPropertyChanged(nameof(IsFileXml));
            NotifyPropertyChanged(nameof(DataFile));
            NotifyPropertyChanged(nameof(FileName));
            NotifyPropertyChanged(nameof(NewFileName));
        }

        /// <summary>
        /// Closes the current file. The user may choose to cancel this operation if there are unsaved changes
        /// </summary>
        /// <param name="_"></param>
        private void CloseCurrentFile(object? _)
        {
            Action closeAction = () =>
            {
                DisableDataRecordEventListeners();
                ResetAllProperties();
                
                Messenger.Send(new ClearUIMessage());
                Messenger.Send(new ClearFocusMessage());
            };

            if (IsDirty)
            {
                Messenger.Send(new ShowYesNoPopupMessage("Close without saving?", "Unsaved changes will be lost.", (yes) =>
                {
                    if (yes)
                    {
                        closeAction();
                    }
                }));
            }
            else
            {
                closeAction();
            }
        }

        private void NavigateToUserManagement(object? _)
        {
            Action navAction = () => Navigate<UserManagementView>();
            if (IsDirty) 
            {
                RequestExit(navAction, "Are you sure you want to leave? Unsaved changes will be lost");
            }
            else
            {
                navAction();
            }
        }

        private void PrepareMenuItems()
        {
            FileMenuItems = new()
            {
                new MenuItem() { Header = "New", Command = NewFileCommand },
                new MenuItem() { Header = "Open", Command = OpenFileCommand },
                new MenuItem() { Header = "Save", Command = SaveCommand },
                new MenuItem() { Header = "Close", Command = CloseCommand },
            };
            EditMenuItems = new()
            {
                new MenuItem() { Header = "Undo", Command = UndoCommand },
                new MenuItem() { Header = "Redo", Command = RedoCommand },
                new MenuItem() { Header = "Rename", Command = EditFileNameCommand },
            };
            HelpMenuItems = new()
            {
                new MenuItem() { Header = "About This Page", Command = ShowAboutCommand }
            };
            ViewMenuItems = new()
            {
                new MenuItem() { Header = "Users", Command = NavigateToUserManagementCommand }
            };
        }

        private void ShowAbout(object? _)
        {
            Messenger.Send(new ShowPopupMessage("About Data Records", "The Data Records page is where you can add, edit and remove data records, as well as load and save them." +
                "\n\nThis page makes use of the command pattern to allow you to undo or redo, as well as restore interrupted work." +
                "\n\nThis page also makes use of the factory pattern to allow you to easily switch between save formats." +
                "\n\nFinally, this page makes use of the proxy pattern, ensuring the current user has adequate rights to perform these actions."));
        }

        #endregion
    }
}
