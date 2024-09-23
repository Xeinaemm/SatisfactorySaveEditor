using SatisfactorySaveEditor.ViewModel;
using System.Windows;

namespace SatisfactorySaveEditor.View;

public partial class PreferencesWindow : Window
{
    public PreferencesWindow(PreferencesWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
