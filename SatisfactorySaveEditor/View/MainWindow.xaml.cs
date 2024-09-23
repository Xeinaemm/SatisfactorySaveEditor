using SatisfactorySaveEditor.Data;
using SatisfactorySaveEditor.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace SatisfactorySaveEditor.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly AppSettingsDbContext appSettingsDbContext;

    public MainWindow(AppSettingsDbContext appSettingsDbContext, MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        var settings = appSettingsDbContext.AppSettings.First();
        Width = settings.WindowWidth;
        Height = settings.WindowHeight;

        if (settings.WindowLeft > 0)
            Left = settings.WindowLeft;
        if (settings.WindowTop > 0)
            Top = settings.WindowTop;
        this.appSettingsDbContext = appSettingsDbContext;
    }

    private void TreeViewSelectedItemChanged(object sender, RoutedEventArgs e)
    {
        if (sender is TreeViewItem item)
        {
            item.BringIntoView();
            e.Handled = true;  
        }
    }

    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        var settings = appSettingsDbContext.AppSettings.First();
        settings.WindowWidth = Width;
        settings.WindowHeight = Height;
        settings.WindowLeft = Left;
        settings.WindowTop = Top;
        appSettingsDbContext.AppSettings.Update(settings);
        appSettingsDbContext.SaveChanges();
    }
}