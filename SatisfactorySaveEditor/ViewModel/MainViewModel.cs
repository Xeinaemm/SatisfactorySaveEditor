using SatisfactorySaveEditor.Model;
using SatisfactorySaveParser;
using System.Collections.ObjectModel;
using System.Globalization;
using SatisfactorySaveEditor.Util;
using System.Windows;
using Microsoft.Win32;
using System.Reflection;
using System.IO;
using GongSolutions.Wpf.DragDrop;
using SatisfactorySaveEditor.Cheats;
using SatisfactorySaveEditor.View;
using System.IO.Compression;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Serilog;
using SatisfactorySaveEditor.Data;

namespace SatisfactorySaveEditor.ViewModel;

public partial class MainViewModel : ObservableObject, IDropTarget
{
    private SatisfactorySave saveGame;
    private SaveObjectModel _rootItem;

    [ObservableProperty]
    private SaveObjectModel selectedItem;

    [ObservableProperty]
    private string searchText;

    private CancellationTokenSource tokenSource = new();

    [ObservableProperty]
    private ObservableCollection<SaveObjectModel> rootItem = [];

    [ObservableProperty]
    private bool isBusy = false;
    private readonly AppSettingsDbContext appSettingsDbContext;
    private readonly PreferencesWindowViewModel preferencesWindowViewModel;
    private readonly StringPromptViewModel stringPromptViewModel;

    public string FileName => saveGame == null ? 
        string.Empty : 
        string.Format(" - {1} [{0}]", saveGame.FileName, saveGame.Header.SessionName);

    partial void OnSearchTextChanged(string value)
    {
        tokenSource.Cancel();
        tokenSource = new CancellationTokenSource();
        Task.Factory.StartNew(() => Filter(value), tokenSource.Token);
    }

    public ObservableCollection<string> LastFiles { get; } = [];

    public ObservableCollection<ICheat> CheatMenuItems { get; } = 
    [
        new ResearchUnlockCheat(),
        new UnlockMapCheat(),
        new RevealMapCheat(),
        new InventorySlotsCheat(), //inventory slot count works again (but is in a different place) as of Update 3
        new ArmSlotsCheat(), //inventory slot count works again (but is in a different place) as of Update 3
        new KillPlayersCheat(),
        new CouponChangerCheat(),
        new DeleteEnemiesCheat(),
        new UndoDeleteEnemiesCheat(),
        new SpawnDoggoCheat(new DeleteEnemiesCheat()),
        new MassDismantleCheat(),
        new EverythingBoxCheat(),
        new CrateSummonCheat(),
        new NoCostCheat(),
        new NoPowerCheat(),
        new RemoveSlugsCheat(),
        new RestoreSlugsCheat(),
        new DeduplicateSchematicsCheat(),
    ];
    public IRelayCommand TreeSelectCommand => new RelayCommand<SaveObjectModel>(SelectNode);
    public IRelayCommand JumpCommand => new RelayCommand<string>(Jump, CanJump);
    public IRelayCommand JumpMenuCommand => new RelayCommand(JumpMenu, () => CanSave(false)); //disallow menu jumping if no save is loaded
    public IRelayCommand ExitCommand => new RelayCommand<CancelEventArgs>(Exit);
    public IRelayCommand OpenCommand => new AsyncRelayCommand<string>(Open);
    public IRelayCommand AboutCommand => new RelayCommand(About);
    public IRelayCommand Help_ViewGithubCommand => new RelayCommand(Help_ViewGithub);
    public IRelayCommand Help_ReportIssueCommand => new RelayCommand(Help_ReportIssue);
    public IRelayCommand Help_RequestHelpDiscordCommand => new RelayCommand(Help_RequestHelpDiscord);
    public IRelayCommand Help_FicsitAppGuideCommand => new RelayCommand(Help_FicsitAppGuide);
    public IRelayCommand DeleteCommand => new RelayCommand<SaveObjectModel>(Delete, CanDelete);
    public IRelayCommand CheatCommand => new RelayCommand<ICheat>(Cheat, CanCheat);
    public IRelayCommand SaveCommand => new AsyncRelayCommand<bool>(Save, CanSave);
    public IRelayCommand ManualBackupCommand => new AsyncRelayCommand(async () => await CreateBackup(true), CanSave);
    public IRelayCommand ResetSearchCommand => new RelayCommand(ResetSearch);
    public IRelayCommand CheckUpdatesCommand => new RelayCommand(() => CheckForUpdate(true).ConfigureAwait(false));
    public IRelayCommand PreferencesCommand => new RelayCommand(OpenPreferences);

    public bool HasUnsavedChanges { get; set; } //TODO: set this to true when any value in WPF is changed. current plan for this according to goz3rr is to make a wrapper for the data from the parser and then change the set method in the wrapper

    public MainViewModel(
        AppSettingsDbContext appSettingsDbContext,
        PreferencesWindowViewModel preferencesWindowViewModel)
    {
        this.appSettingsDbContext = appSettingsDbContext;
        this.preferencesWindowViewModel = preferencesWindowViewModel;
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1 && File.Exists(args[1]))
        {
            var task = LoadFile(args[1]);
            task.Wait();
        }

        var settings = appSettingsDbContext.AppSettings.First();
        
        if (settings.LastSaves != null)
        {
            var modified = false;
            foreach (var filePath in settings.LastSaves) //silently remove files that no longer exist from the list in Properties
            {
                if (!File.Exists(filePath))
                {
                    modified = true;
                    Log.Information($"Removing save file {filePath} from recent saves list since it wasn't found on disk");
                    settings.LastSaves.Remove(filePath);
                }
            }
            if (modified) //regenerate list since a save was not found when first built
            {
                appSettingsDbContext.AppSettings.Update(settings);
                appSettingsDbContext.SaveChanges();
            }
            LastFiles = new ObservableCollection<string>(settings.LastSaves);
        } 
        else //create a new empty collection for the list since there isn't anything there
            LastFiles = [];

        CheckForUpdate(false).ConfigureAwait(false);
    }

    private void OpenPreferences()
    {
        var window = new PreferencesWindow(preferencesWindowViewModel)
        {
            Owner = Application.Current.MainWindow
        };

        window.ShowDialog();
    }

    private async Task CheckForUpdate(bool manual)
    {
        var settings = appSettingsDbContext.AppSettings.First();
        if (!manual && !settings.AutoUpdate)
            return;

        var latestVersion = await UpdateChecker.GetLatestReleaseInfo();

        if (latestVersion.IsNewer())
        {
            var window = new UpdateWindow
            {
                DataContext = new UpdateWindowViewModel(latestVersion),
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();
        }
        else if (manual)
        {
            MessageBox.Show("You are already using the latest version.", "Update", MessageBoxButton.OK);
        }
    }

    /// <summary>
    /// Checks if the passed model is not the rootItem of the save
    /// </summary>
    /// <param name="model"></param>
    /// <returns>If deletion is allowed</returns>
    private bool CanDelete(SaveObjectModel model) => model != _rootItem;

    /// <summary>
    /// Removes the passed model from rootItem and raises property changed on the root item.
    /// </summary>
    /// <param name="model">The model to delete</param>
    private void Delete(SaveObjectModel model)
    {
        RootItem.Remove(model);
        OnPropertyChanged(nameof(RootItem));
    }

    /// <summary>
    /// Checks if rootItem exists (if a save file is opened)
    /// </summary>
    /// <param name="cheat"></param>
    /// <returns>True if the root item is NOT null, false otherwise</returns>
    private bool CanCheat(ICheat cheat) => RootItem != null;

    /// <summary>
    /// Calls Apply() on the passed ICheat, providing it with rootItem. Only mark for unsaved changes if the cheat succeeds.
    /// </summary>
    /// <param name="cheat">The cheat to run</param>
    private void Cheat(ICheat cheat)
    {

        Log.Information($"Applying cheat {cheat.Name}");
        if (cheat.Apply(_rootItem, saveGame))
            HasUnsavedChanges = true;
    }

    /// <summary>
    /// Checks if the editor can perform a save operation
    /// </summary>
    /// <param name="saveAs">If the save operation is Save As (unused)</param>
    /// <returns>True if saveGame is NOT null, false otherwise</returns>
    private bool CanSave(bool _) => saveGame != null;

    private bool CanSave() //overload of CanSave(bool saveAs) for contexts when saveAs doesn't matter
=> CanSave(false);

    /// <summary>
    /// Save changes, creating a backup first if auto backups are enabled in the user's preferences
    /// </summary>
    /// <param name="saveAs">(optional) If the Save As... option box should be brought up to choose a destination</param>
    private async Task Save(bool saveAs)
    {
        if (saveAs)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Satisfactory save file|*.sav",
                InitialDirectory = Path.GetDirectoryName(saveGame.FileName),
                DefaultExt = ".sav",
                CheckFileExists = false,
                AddExtension = true
            };

            if (dialog.ShowDialog() == true)
            {
                await AutoBackupIfEnabled();

                var newObjects = _rootItem.DescendantSelf;
                saveGame.Entries = saveGame.Entries.Intersect(newObjects).ToList();
                saveGame.Entries.AddRange(newObjects.Except(saveGame.Entries));

                _rootItem.ApplyChanges();
                IsBusy = true;
                await Task.Run(() => saveGame.Save(dialog.FileName));
                IsBusy = false;
                HasUnsavedChanges = false;
                OnPropertyChanged(nameof(FileName));
                AddRecentFileEntry(dialog.FileName);
            }
        }
        else
        {
            await AutoBackupIfEnabled();

            var newObjects = _rootItem.DescendantSelf;
            saveGame.Entries = saveGame.Entries.Intersect(newObjects).ToList();
            saveGame.Entries.AddRange(newObjects.Except(saveGame.Entries));

            _rootItem.ApplyChanges();
            IsBusy = true;
            await Task.Run(saveGame.Save);
            IsBusy = false;
            HasUnsavedChanges = false;
        }
    }

    private async Task AutoBackupIfEnabled()
    {
        var settings = appSettingsDbContext.AppSettings.First();
        if (settings.AutoBackup)
            await CreateBackup(false);
    }

    private async Task CreateBackup(bool manual)
    {
        IsBusy = true;
        await Task.Run(() => CreateBackupAsync(manual));
        IsBusy = false;
    }

    private void CreateBackupAsync(bool manual)
    {  
        var saveFileDirectory = Path.GetDirectoryName(saveGame.FileName);
        var tempDirectoryName = @"\SSEtemp\";
        var pathToZipFrom = saveFileDirectory + tempDirectoryName;

        var tempFilePath = saveFileDirectory + tempDirectoryName + Path.GetFileName(saveGame.FileName);
        var backupFileFullPath = saveFileDirectory + @"\" + Path.GetFileNameWithoutExtension(saveGame.FileName) + "_" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + ".SSEbkup.zip";

        Log.Information($"Creating a {(manual ? "manual " : "")}backup for {saveGame.FileName}");

        try
        {
            //Satisfactory save files compress exceedingly well, so compress all backups so that they take up less space.
            //ZipFile only accepts directories, not single files, so copy the save to a temporary folder and then zip that folder
            Directory.CreateDirectory(pathToZipFrom);
            File.Copy(saveGame.FileName, tempFilePath, true); 
            ZipFile.CreateFromDirectory(pathToZipFrom, backupFileFullPath);
        }
        catch (Exception ex)
        {
            //should never be reached, but hopefully any users that encounter an error here will report it 
            MessageBox.Show("An error occurred while creating a backup. The error message will appear when you press 'Ok'.\nPlease tell Goz3rr, Robb, or virusek20 the contents of the error, or report it on the Github Issues page with your log file and save file attached.");
            Log.Error(ex.StackTrace);
            throw;
        }
        finally
        {
            //delete the temporary folder and copy even if the zipping process fails
            File.Delete(tempFilePath);
            Directory.Delete(pathToZipFrom);
        }

        if (manual)
            MessageBox.Show("Backup created. Find it in your save file folder.");
    }

    /// <summary>
    /// Checks if it's possible to jump to the passed EntityName string
    /// </summary>
    /// <param name="target">The EntityName to jump to, in string format</param>
    /// <returns>True if rootItem contains the EntitiyName, false otherwise.</returns>
    private bool CanJump(string target) => _rootItem.FindChild(target, false) != null;

    /// <summary>
    /// Opens the Github repo page scrolled to the 'Help' heading
    /// </summary>
    private static void Help_ViewGithub() => Process.Start("https://github.com/Goz3rr/SatisfactorySaveEditor#help");

    /// <summary>
    /// Opens the Github repo page scrolled to the Issues tab
    /// </summary>
    private static void Help_ReportIssue() => Process.Start("https://github.com/Goz3rr/SatisfactorySaveEditor/issues");

    /// <summary>
    /// Notifies the user of their redirection to the discord, then opens the invite.
    /// </summary>
    private static void Help_RequestHelpDiscord()
    {
        MessageBox.Show("You are now being redirected to the Satisfactory Modding discord server. Please request help in the #savegame-edits channel.");
        Process.Start("https://bit.ly/SatisfactoryModding"); //discord invite for Satisfactory Modding server. Contact BaineGames#7333 if it breaks
    }

    /// <summary>
    /// Notifies the user of their redirection to the ficsit.app guide, then opens the guide.
    /// </summary>
    private static void Help_FicsitAppGuide()
    {
        MessageBox.Show("You are now being redirected to the ficsit.app mod and tool repository to view a guide.");
        Process.Start("https://ficsit.app/guide/Z8h6z2CczH43c");
    }


    /// <summary>
    /// Displays version information box
    /// </summary>
    private static void About()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        MessageBox.Show($"Satisfactory save editor{Environment.NewLine}{version}", "About");
    }

    /// <summary>
    /// Starts the process of loading a file, prompting the user if there are unsaved changes. Marks as having no unsaved changes
    /// </summary>
    /// <param name="fileName">Path to the save file</param>
    private async Task Open(string fileName)
    {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            await LoadFile(fileName);
            HasUnsavedChanges = false;

            return;
        }

        if (HasUnsavedChanges)
        {
            var result = MessageBox.Show("You have unsaved changes. Abandon changes by opening another file?\n\nNote: Changes made in the data text fields are not yet tracked as saved/unsaved but are still saved.", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                return;
            }
        }

        //TODO: swap this over to calling the save method instead
        var dialog = new OpenFileDialog
        {
            Filter = "Satisfactory save file|*.sav"
        };

        var newPath = Environment.ExpandEnvironmentVariables(@"%localappdata%\FactoryGame\Saved\SaveGames\");
        var oldPath = Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents\My Games\FactoryGame\SaveGame\");

        dialog.InitialDirectory = Directory.Exists(newPath) ? newPath : oldPath;

        if (dialog.ShowDialog() == true)
        {
            await LoadFile(dialog.FileName);
            HasUnsavedChanges = false;
        }
    }

    /// <summary>
    /// Checks if there are unsaved changes, exits otherwise or if the user choses to discard.
    /// TODO: Mark as unsaved when property fileds are changed
    /// TODO: Check this when pressing alt+f4 and clicking the red x
    /// </summary>
    private void Exit(CancelEventArgs args = null)
    {
        if (HasUnsavedChanges)
        {
            var result = MessageBox.Show("You have unsaved changes. Close and abandon changes?\n\nNote: Changes made in the data text fields are not yet tracked as saved/unsaved but are still saved.", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
            else
                if (args != null)
                    args.Cancel = true;
        }
        else
        {
            Application.Current.Shutdown();
        }
    }

    /// <summary>
    /// Select the specified entity in the tree view
    /// </summary>
    /// <param name="target">EntityName of the entity to jump to</param>
    private void Jump(string target)
    {
        if(SelectedItem != null)
            SelectedItem.IsSelected = false;
        SelectedItem = _rootItem.FindChild(target, true);
    }

    /// <summary>
    /// Opens a StringPromptWindow prompting for an EntityName to jump to
    /// </summary>
    private void JumpMenu()
    {
        var dialog = new StringPromptWindow()
        {
            Owner = Application.Current.MainWindow
        };
        var cvm = (StringPromptViewModel)dialog.DataContext;
        cvm.WindowTitle = "Jump to Tag";
        cvm.PromptMessage = "Tag name:";
        cvm.ValueChosen = "";
        cvm.OldValueMessage = "Obtain via Right Click > Copy name\nExample:\nPersistent_Level:PersistentLevel.Char_Player_C_0.inventory";
        dialog.ShowDialog();

        var destination = cvm.ValueChosen;

        if(!(destination.Equals("") || destination.Equals("cancel")))
            if (CanJump(destination))
                Jump(destination);
            else
                MessageBox.Show("Failed to jump to tag:\n" + destination);
    }

    /// <summary>
    /// Selects a node
    /// </summary>
    /// <param name="node">The node to select</param>
    private void SelectNode(SaveObjectModel node) => SelectedItem = node;

    /// <summary>
    /// Loads a file into the editor
    /// </summary>
    /// <param name="path">The path to the file to open</param>
    private async Task LoadFile(string path)
    {
        SelectedItem = null;
        SearchText = null;

        IsBusy = true;
        await Task.Run(() => LoadFileAsync(path));
        IsBusy = false;
    }

    private void LoadFileAsync(string path)
    {
        try
        {
            saveGame = new SatisfactorySave(path);
        }
        catch (FileNotFoundException)
        {
            if (LastFiles != null && LastFiles.Contains(path)) //if the save file that failed to open was on the last found list, remove it. this should only occur when people move save files around and leave the editor open.
            {
                MessageBox.Show("That file could no longer be found on the disk.\nIt has been removed from the recent files list.", "File not present", MessageBoxButton.OK, MessageBoxImage.Warning);
                Log.Information($"Removing save file {path} from recent saves list since it wasn't found on disk");
                var settings = appSettingsDbContext.AppSettings.First();
                settings.LastSaves.Remove(path);
                appSettingsDbContext.AppSettings.Update(settings);
                Application.Current.Dispatcher.Invoke(() => LastFiles.Remove(path));
            }
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while opening the file:\n{ex.Message}\n\nCheck the logs for more details.\n\nIf this issue persists, please report it via \"Help > Report an Issue\", and attach the log file and save file you were trying to open.", "Error opening file", MessageBoxButton.OK, MessageBoxImage.Error);
            Log.Error(ex.StackTrace);
            return;
        }
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            //manually raise these for the AsyncCommand library to pick up on it (ask virusek20 or Robb)
            SaveCommand.NotifyCanExecuteChanged();
            ManualBackupCommand.NotifyCanExecuteChanged();
        });

        _rootItem = new SaveRootModel(saveGame.Header);
        var saveTree = new EditorTreeNode("Root");

        foreach (var entry in saveGame.Entries)
        {
            var parts = entry.TypePath.TrimStart('/').Split('/');
            saveTree.AddChild(parts, entry);
        }

        BuildNode(RootItem, saveTree);

        _rootItem.IsExpanded = true;
        foreach (var item in RootItem)
        {
            item.IsExpanded = true;
        }

        OnPropertyChanged(nameof(RootItem));
        OnPropertyChanged(nameof(FileName));

        AddRecentFileEntry(path);
    }

    /// <summary>
    /// Adds a recently opened file to the list
    /// </summary>
    /// <param name="path">The path of the file to add</param>
    private void AddRecentFileEntry(string path)
    {
        var settings = appSettingsDbContext.AppSettings.First();

        settings.LastSaves ??= [];

        if (LastFiles.Contains(path)) // No duplicates
        {
            settings.LastSaves.Remove(path);
            Application.Current.Dispatcher.Invoke(() => LastFiles.Remove(path));
            
        }

        settings.LastSaves.Add(path);
        Application.Current.Dispatcher.Invoke(() =>
        {
            LastFiles.Add(path);

            while (settings.LastSaves.Count >= 6) // Keeps only 5 most recent saves
            {
                LastFiles.RemoveAt(0);
                settings.LastSaves.RemoveAt(0);
            }
        });

        appSettingsDbContext.AppSettings.Update(settings);

        Application.Current.Dispatcher.Invoke(() =>
        {
            RootItem.Clear();
            RootItem.Add(_rootItem);
        });
    }

    private static void BuildNode(ObservableCollection<SaveObjectModel> items, EditorTreeNode node)
    {
        foreach (var child in node.Children)
        {
            var childItem = new SaveObjectModel(child.Value.Name);
            BuildNode(childItem.Items, child.Value);
            items.Add(childItem);
        }

        foreach (var entry in node.Content)
        {
            switch (entry)
            {
                case SaveEntity se:
                    items.Add(new SaveEntityModel(se));
                    break;
                case SaveComponent sc:
                    items.Add(new SaveComponentModel(sc));
                    break;
            }
        }
    }

    private void Filter(string value)
    {
        if (_rootItem == null)
            return;

        if (string.IsNullOrWhiteSpace(value))
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                RootItem.Clear();
                RootItem.Add(_rootItem);
            });
        }
        else
        {
            var valueLower = value.ToLower(CultureInfo.InvariantCulture);
            var filter = _rootItem.DescendantSelfViewModel.WithCancellation(tokenSource.Token).Where(vm => vm.MatchesFilter(valueLower));
            Application.Current.Dispatcher.Invoke(() => RootItem = new ObservableCollection<SaveObjectModel>(filter));
        }
    }

    private void ResetSearch() => SearchText = null;

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is not DataObject data)
            return;

        var files = data.GetFileDropList();
        if (files == null || files.Count == 0)
            return;

        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Copy;
    }

    /// <summary>
    /// Handle drag and drop opening of save files
    /// </summary>
    /// <param name="dropInfo"></param>
    public void Drop(IDropInfo dropInfo)
    {
        var fileName = ((DataObject)dropInfo.Data).GetFileDropList()[0];
        _ = LoadFile(fileName);
        // No need to wait for it to finish, since that blocks the dispatcher thread and causes a deadlock
    }
}