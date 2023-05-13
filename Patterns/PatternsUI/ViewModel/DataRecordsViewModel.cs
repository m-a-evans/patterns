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
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PatternsUI.ViewModel
{
    /// <summary>
    /// Handles the actual data record management of the application, such as loading, saving, editing and deleting records
    /// </summary>
    public class DataRecordsViewModel : ViewModelBase
    {
        #region Fields

        private const int EmptyCommandHistoryIndex = -1;
        private List<DataCommand> _commandHistory = new List<DataCommand>();
        private int _commandHistoryIndex = EmptyCommandHistoryIndex;
        private int _commandHistorySaveIndex = 0;

        private DataFile? _currentFile;

        private readonly object _saveHistoryLock = new();

        private string _fileNameFull = string.Empty;

        private const string PatternzCommandListName = ".pcl";
        private const string DefaultDirectory = "../../data";

        #endregion

        #region Properties

        public DataFile? DataFile { get { return _currentFile; } }

        public bool IsDirty = false;

        public bool IsFileXml { get; set; } = false;
        public bool IsFileLoaded { get => _currentFile != null; }
        public bool IsFileNew { get; set; } = false;
        public bool IsEditingFileName { get; set; } = false;
        public string NewFileName { get; set; } = string.Empty;
        public string FileName { get => Path.GetFileName(_fileNameFull); } 

        public RelayCommand EditFileNameCommand { get; private set; }
        public RelayCommand XmlSelectedCommand { get; private set; }
        public RelayCommand JsonSelectedCommand { get; private set; }
        public RelayCommand NavigateToUserManagementCommand { get; private set; }
        public RelayCommand ShowAboutCommand { get; private set; }
        public RelayCommand NewCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
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
            ShowAboutCommand = new RelayCommand(ShowAbout);
            NavigateToUserManagementCommand = new RelayCommand(NavigateToUserManagement);
            NewCommand = new RelayCommand(NewFile);
            OpenCommand = new RelayCommand(OpenFile);
            SaveCommand = new RelayCommand(SaveData, CanSaveCommandExecute);
            CloseCommand = new RelayCommand(CloseCurrentFile, CanCloseCommandExecute);
            UndoCommand = new RelayCommand(Undo, CanUndoCommandExecute);
            RedoCommand = new RelayCommand(Redo, CanRedoCommandExecute);
            RenameCommand = new RelayCommand(RenameFile);
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
                    if (confirmed) exit();
                });
                Messenger.Send(ConfirmExitMessage);
            }
            else
            {
                exit();
            }
        }

        #endregion

        #region Private Methods
        
        private void CreateNewDataFile(object? _)
        {
            IsDirty = true;
            _currentFile = new DataFile();
            _currentFile.Format = DataRecordFormat.Xml;
            _currentFile.FileName = FileName;
            _currentFile.Path = DefaultDirectory;
            SaveData(null);
        }

        private bool CanCloseCommandExecute(object? _)
        {
            return IsFileLoaded;
        }

        private bool CanRedoCommandExecute(object? _)
        {
            return _commandHistory.Count - 1 > _commandHistoryIndex;
        }

        private bool CanUndoCommandExecute(object? _)
        {
            return _commandHistory.Count > 0;
        }

        private void PushToCommandHistory(DataCommand cmd)
        {
            if (_commandHistory.Count - 1 == _commandHistoryIndex)
            {
                _commandHistory.Add(cmd);
            }
            else
            {
                _commandHistory[_commandHistoryIndex] = cmd;
            }
            _commandHistoryIndex += 1;
            SaveCommandHistory();
        }

        private void CheckForRecoveryFile()
        {
            if (!File.Exists(PatternzCommandListName))
            {
                return;
            }

            CommandHistoryMetadata? metadata = JsonSerializer.Deserialize<CommandHistoryMetadata>(PatternzCommandListName);

            if (metadata == null)
            {
                return;
            }

            if (!File.Exists(metadata.FileName))
            {
                return;
            }
            string justFileName = Path.GetFileName(metadata.FileName);
            Messenger.Send(new ShowYesNoPopupMessage("Recover file?", $"Patternz has detected that {justFileName} has changes that can be recovered. Proceed?", 
                (yes) =>
                {
                    if (yes)
                    {
                        RecoverFile(metadata);
                    }
                })
            );
        }

        private void RecoverFile(CommandHistoryMetadata metadata)
        {
            LoadFileByName(metadata.FileName);
            _commandHistory = metadata.History;
            _commandHistoryIndex = EmptyCommandHistoryIndex;
            // Redo all commands stored in the history 
            while(CanRedoCommandExecute(null))
            {
                Redo(null);
            }
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
            IDataRecordManager dataRecordManager = Coordinator.Instance.GetDataRecordManager(IsFileXml ? DataRecordFormat.Xml : DataRecordFormat.Json);
            long result = dataRecordManager.WriteDataRecords(_currentFile);

            _commandHistorySaveIndex = _commandHistoryIndex;

            IsDirty = false;
            NotifyAllProperties();
        }

        private void NewFile(object? _)
        {
            ResetAllProperties();
            _currentFile = new DataFile();
            IsEditingFileName = true;
            IsFileNew = true;
            NotifyPropertyChanged(nameof(IsEditingFileName));
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
            dataRecordManager.TryParseRecords(fileName, out _currentFile);

            if (_currentFile != null)
            {
                IsFileNew = false;
                IsFileXml = _currentFile.Format == DataRecordFormat.Xml;
                IsDirty = false;
                IsEditingFileName = false;
                _fileNameFull = _currentFile.Path + _currentFile.FileName;
                NotifyAllProperties();
            }
        }

        /// <summary>
        /// Resets all properties to their default values, and clears command history
        /// </summary>
        private void ResetAllProperties()
        {
            IsDirty = false;
            IsEditingFileName = false;
            IsFileNew = false;
            IsFileXml = false;

            _currentFile = null;
            _commandHistory.Clear();
            _commandHistoryIndex = EmptyCommandHistoryIndex;
            _commandHistorySaveIndex = 0;

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
        }

        /// <summary>
        /// Closes the current file. The user may choose to cancel this operation if there are unsaved changes
        /// </summary>
        /// <param name="_"></param>
        private void CloseCurrentFile(object? _)
        {
            Action closeAction = () =>
            {
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

        /// <summary>
        /// Executes the undone command that was after the most recently used command.
        /// </summary>
        /// <param name="_"></param>
        private void Redo(object? _)
        {
            _commandHistoryIndex += 1;
            DataCommand command = _commandHistory[_commandHistoryIndex];
            command.Execute();
            SaveCommandHistory();
        }

        private void Undo(object? _)
        {
            DataCommand command = _commandHistory[_commandHistoryIndex];
            command.Unexecute();
            _commandHistoryIndex -= 1;
            SaveCommandHistory();
        }

        private void SaveCommandHistory()
        {
            Task saveCmd = Task.Run(() =>
            {
                lock (_saveHistoryLock)
                {
                    File.WriteAllText(PatternzCommandListName,
                    JsonSerializer.Serialize(new CommandHistoryMetadata()
                    {
                        FileName = FileName,
                        // Get all commands executed since last Save
                        History = _commandHistory.GetRange(_commandHistorySaveIndex, _commandHistoryIndex + 1)
                    })) ;
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
                new MenuItem() { Header = "New", Command = NewCommand },
                new MenuItem() { Header = "Open", Command = OpenCommand },
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
