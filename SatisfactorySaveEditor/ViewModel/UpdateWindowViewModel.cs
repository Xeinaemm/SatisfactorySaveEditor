using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactorySaveEditor.Data;
using SatisfactorySaveEditor.Util;
using System.Diagnostics;
using System.Windows;

namespace SatisfactorySaveEditor.ViewModel;

public class UpdateWindowViewModel : ObservableObject
{
    public UpdateWindowViewModel(AppSettingsDbContext appSettingsDbContext)
        => this.appSettingsDbContext = appSettingsDbContext;

    private readonly UpdateChecker.VersionInfo info;
    private readonly AppSettingsDbContext appSettingsDbContext;

    public IRelayCommand OpenCommand => new RelayCommand<Window>(Open);
    public IRelayCommand CloseCommand => new RelayCommand<Window>(Close);
    public IRelayCommand DisableAutoCheckCommand => new RelayCommand<Window>(DisableAutoCheck);

    public string Changelog => $"Satisfactory Save Editor {info.TagName}"
        + Environment.NewLine + info.Name + Environment.NewLine + Environment.NewLine + info.Changelog;

    public UpdateWindowViewModel(UpdateChecker.VersionInfo info) => this.info = info;

    private static void Close(Window window) => window.Close();

    private void DisableAutoCheck(Window window)
    {
        var settings = appSettingsDbContext.AppSettings.First();
        settings.AutoUpdate = false;
        _ = appSettingsDbContext.AppSettings.Update(settings);
        _ = appSettingsDbContext.SaveChanges();
        _ = MessageBox.Show("You have disabled automatic update checking. You can re-enable it in the preferences menu or manually check for updates in the 'Help' menu.", "Update", MessageBoxButton.OK);
        window.Close();
    }

    private void Open(Window window)
    {
        _ = Process.Start(info.ReleaseUrl);
        window.Close();
    }
}
