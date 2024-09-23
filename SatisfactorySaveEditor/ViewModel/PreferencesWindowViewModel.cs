using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactorySaveEditor.Data;
using System.Windows;

namespace SatisfactorySaveEditor.ViewModel;

public partial class PreferencesWindowViewModel : ObservableObject
{
    public PreferencesWindowViewModel(AppSettingsDbContext appSettingsDbContext)
        => this.appSettingsDbContext = appSettingsDbContext;

    [ObservableProperty]
    private bool canApply;

    [ObservableProperty]
    private bool autoUpdate;

    [ObservableProperty]
    private bool autoBackup;

    private readonly AppSettingsDbContext appSettingsDbContext;

    partial void OnAutoUpdateChanged(bool value)
    {
        CanApply = true;
        OnPropertyChanged(nameof(CanApply));
    }

    partial void OnAutoBackupChanged(bool value)
    {
        CanApply = true;
        OnPropertyChanged(nameof(CanApply));
    }

    public IRelayCommand AcceptCommand => new RelayCommand<Window>(Accept);
    public IRelayCommand ApplyCommand => new RelayCommand(Apply);
    public IRelayCommand CancelCommand => new RelayCommand<Window>(Cancel);

    public PreferencesWindowViewModel()
    {
        var settings = appSettingsDbContext.AppSettings.First();
        autoUpdate = settings.AutoUpdate;
        autoBackup = settings.AutoBackup;
    }

    private void Accept(Window window)
    {
        Apply();
        window.Close();
    }

    private void Apply()
    {
        var settings = appSettingsDbContext.AppSettings.First();
        settings.AutoUpdate = AutoUpdate;
        settings.AutoBackup = AutoBackup;
        appSettingsDbContext.AppSettings.Update(settings);
        appSettingsDbContext.SaveChanges();
        CanApply = true;
        OnPropertyChanged(nameof(CanApply));
    }

    private static void Cancel(Window window) => window.Close();
}
