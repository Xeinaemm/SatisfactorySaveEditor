using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

//TODO: Make pressing enter in the text box trigger Ok()
namespace SatisfactorySaveEditor.ViewModel;

public partial class StringPromptViewModel : ObservableObject
{
    public IRelayCommand OkCommand => new RelayCommand<Window>(Ok);
    public IRelayCommand CancelCommand => new RelayCommand<Window>(Cancel);

    [ObservableProperty]
    private string windowTitle = "String Prompt";

    [ObservableProperty]
    private string promptMessage = "Prompt String:";

    [ObservableProperty]
    private string valueChosen;

    [ObservableProperty]
    private string oldValueMessage = "(1)Old Value Message\n(2)\n(3)\n(4)\n(5)";

    private void Cancel(Window obj)
    {
        ValueChosen = "cancel";
        obj.Close();
    }

    private static void Ok(Window obj) => obj.Close();
}
