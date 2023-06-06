using CommunityToolkit.Diagnostics;
using Microsoft.Win32;
using Patterns;
using Patterns.Command;
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
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
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

        private const string PatternzCommandListName = ".pcl";
        private const string DefaultDirectory = "../../../../data";

        private ObservableCollection<DataRecord> _dataRecords = new();

        private Tuple<string, DataRecord> _propertyUpdatingHelper;

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

        public bool IsFileXml { get; set; } = false;
        public bool IsFileLoaded { get => _currentFile != null; }
        public bool IsFileNew { get; set; } = false;
        public bool IsEditingFileName { get; set; } = false;
        public string NewFileName { get; set; } = string.Empty;
        public string FileName { get => _currentFile?.FileName != null ? Path.GetFileName(_currentFile.FileName) : string.Empty; } 

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
        public RelayCommand RenameCommand { get; private set; }
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
            RenameCommand = new RelayCommand(RenameFile);
            EditFileNameCommand = new RelayCommand(SetEditFileNameMode);
            XmlSelectedCommand = new RelayCommand(SelectXmlFormat);
            JsonSelectedCommand = new RelayCommand(SelectJsonFormat);
            SaveFileNameCommand = new RelayCommand(CreateNewDataFile);

            _dataRecords.CollectionChanged += OnDataRecordCollectionChanged;

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
                        CleanupEventListeners();
                        exit();
                    }                        
                });
                Messenger.Send(ConfirmExitMessage);
            }
            else
            {
                CleanupEventListeners();
                exit();
            }
        }

        #endregion

        #region Private Methods
        
        private void CleanupEventListeners()
        {
            _dataRecords.CollectionChanged -= OnDataRecordCollectionChanged;
            RemoveAllDataRecordListeners();
        }

        private void RemoveAllDataRecordListeners()
        {
            foreach (DataRecord record in _dataRecords) 
            {
                record.PropertyChanged -= OnDataRecordChanged;
                record.PropertyChanging -= OnDataRecordChanging;
            }
        }

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
                    if (_commandHistory.LastExecutedCommand is CreateDataRecordCommand createCmd 
                        && createCmd.State == CommandState.Unexecuted)
                    {
                        // If this data record is being removed from undoing a create command, don't add to history
                        // as a remove command
                        return;
                    }
                    PushToCommandHistory(new RemoveDataRecordCommand(_dataRecords, new RemoveDataRecordParam(record)));
                }
            }
        }

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

        private void OnDataRecordChanged(object? sender, PropertyChangedEventArgs e) 
        {
            // TODO - see above TODO
            if (sender is DataRecord record)
            {
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
                else if (_commandHistory.LastExecutedCommand?.State != CommandState.Unexecuted)
                {
                    PushToCommandHistory(new EditDataRecordCommand(_dataRecords, new EditDataRecordParam(record, _oldRecord?.DeepCopy())));
                }
            }
        }

        private void CreateNewDataFile(object? name)
        {
            string fileName = name as string ?? NewFileName;
            _currentFile = new DataFile();
            _currentFile.Format = IsFileXml ? DataRecordFormat.Xml : DataRecordFormat.Json;            
            _currentFile.Path = DefaultDirectory;
            _currentFile.FileName = _currentFile.Path + "/" + AppendFileExtensionIfAbsent(fileName, _currentFile.Format);
            _commandHistory.FileName = _currentFile.FileName;
            SaveData(null);
        }

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

        private bool CanCloseCommandExecute(object? _)
        {
            return IsFileLoaded || IsEditingFileName;
        }

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
            _commandHistory.Redo();
            //SaveCommandHistory();
        }

        private bool CanUndoCommandExecute(object? _)
        {
            return _commandHistory.CanUndo();
        }

        private void UndoLastCommand(object? _)
        {
            _commandHistory.Undo();
            //SaveCommandHistory();
        }

        private void PushToCommandHistory(DataCommand cmd)
        {
            _commandHistory.AddCommand(cmd);
            //SaveCommandHistory();
        }

        private void CheckForRecoveryFile()
        {
            if (!File.Exists(PatternzCommandListName))
            {
                return;
            }

            CommandHistory? commandHistory = JsonSerializer.Deserialize<CommandHistory>(PatternzCommandListName);

            if (commandHistory == null)
            {
                return;
            }

            if (!File.Exists(commandHistory.FileName))
            {
                return;
            }
            string justFileName = Path.GetFileName(commandHistory.FileName);
            Messenger.Send(new ShowYesNoPopupMessage("Recover file?", $"Patternz has detected that {justFileName} has changes that can be recovered. Proceed?", 
                (yes) =>
                {
                    if (yes)
                    {
                        RecoverFile(commandHistory);
                    }
                })
            );
        }

        private void RecoverFile(CommandHistory commandHistory)
        {
            LoadFileByName(commandHistory.FileName);
            _commandHistory = commandHistory;
            _commandHistory.ExecuteEntireHistory();
        }

        private void SetEditFileNameMode(object? _)
        {
            IsEditingFileName = true;
            NotifyPropertyChanged(nameof(IsEditingFileName));
        }

        private void SelectJsonFormat(object? _)
        {
            if (IsFileXml)
            {
                IsFileXml = false;
                SetDataRecordFormatCommand cmd = new(DataFile);

                cmd.Execute(new SetDataRecordFormatParam(DataRecordFormat.Json));
                PushToCommandHistory(cmd);
            }
        }

        private void SelectXmlFormat(object? _)
        {
            if (!IsFileXml)
            {
                IsFileXml = true;
                SetDataRecordFormatCommand cmd = new(DataFile);

                cmd.Execute(new SetDataRecordFormatParam(DataRecordFormat.Xml));
                PushToCommandHistory(cmd);
            }
        }

        private bool CanSaveCommandExecute(object? _)
        {
            return IsDirty && IsFileLoaded;
        }

        private void SaveData(object? _)
        {
            if (_currentFile == null)
            {
                ThrowHelper.ThrowInvalidOperationException("Unable to save data. Current File is null");
            }

            IDataRecordManager dataRecordManager = 
                Coordinator.Instance.GetDataRecordManager(IsFileXml ? DataRecordFormat.Xml : DataRecordFormat.Json);
            CopyRecordsToFile(_currentFile, DataRecords);
            _ = dataRecordManager.WriteDataRecords(_currentFile);

            _commandHistorySaveIndex = _commandHistory.CurrentIndex;

            IsEditingFileName = false;
            NotifyAllProperties();
        }

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

        private void CopyRecordsFromFile(DataFile file, ICollection<DataRecord> records)
        {
            records.Clear();
            foreach (DataRecord record in file.DataRecords.Values)
            {                
                records.Add(record);
            }
        }

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

        private void LoadFileByName(string fileName)
        {
            _currentFile = null;
            IDataRecordManager dataRecordManager = Coordinator.Instance.GetDataRecordManager(fileName);
            _ = dataRecordManager.TryParseRecords(fileName, out _currentFile);

            if (_currentFile != null)
            {
                IsFileNew = false;
                IsFileXml = _currentFile.Format == DataRecordFormat.Xml;
                IsEditingFileName = false;
                CopyRecordsFromFile(_currentFile, _dataRecords);
                foreach(DataRecord record in _dataRecords)
                {
                    record.PropertyChanging += OnDataRecordChanging;
                    record.PropertyChanged += OnDataRecordChanged;
                }
                // Clear command history, since technically loading the records from file
                // counts as create commands
                _commandHistory.Reset();
                _commandHistorySaveIndex = _commandHistory.CurrentIndex;
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
            IsFileXml = false;

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
            NotifyPropertyChanged(nameof(IsFileLoaded));
            NotifyPropertyChanged(nameof(IsFileNew));
            NotifyPropertyChanged(nameof(IsFileXml));
            NotifyPropertyChanged(nameof(DataFile));
            NotifyPropertyChanged(nameof(FileName));
        }

        /// <summary>
        /// Closes the current file. The user may choose to cancel this operation if there are unsaved changes
        /// </summary>
        /// <param name="_"></param>
        private void CloseCurrentFile(object? _)
        {
            Action closeAction = () =>
            {
                RemoveAllDataRecordListeners();
                ResetAllProperties();

                // Delete command history file
                File.Delete(PatternzCommandListName);
                
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

        private void SaveCommandHistory()
        {
            Task saveCmd = Task.Run(() =>
            {
                lock (_saveHistoryLock)
                {
                    File.WriteAllText(PatternzCommandListName,
                    JsonSerializer.Serialize(new CommandHistory()
                    {
                        FileName = FileName,
                        // Get all commands executed since last Save
                        // TODO - make this store a list of commands in order from here since last save
                        History = _commandHistory.GetRelativeHistory(_commandHistorySaveIndex)
                    })); ;
                }
            }).ContinueWith((t) => {
                if (t.IsFaulted)
                {
                    Debug.WriteLine(t.Exception?.Message);
                }
            });
        }

        private void RenameFile(object? _)
        {

        }

        private void NavigateToUserManagement(object? _)
        {
            Action navAction = () => Navigate<UserManagementView>();
            if (IsDirty) 
            {
                RequestExit(navAction, "Are you sure you want to leave?");
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
                new MenuItem() { Header = "Rename", Command = RenameCommand },
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
