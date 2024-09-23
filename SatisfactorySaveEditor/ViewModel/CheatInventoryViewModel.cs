using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace SatisfactorySaveEditor.ViewModel;

public partial class CheatInventoryViewModel : ObservableObject
{
    public IRelayCommand OkCommand => new RelayCommand<Window>(Ok);
    public IRelayCommand CancelCommand => new RelayCommand<Window>(Cancel);

    [ObservableProperty]
    private int numberChosen;

    [ObservableProperty]
    private int oldSlotsDisplay;

    private void Cancel(Window obj)
    {
        NumberChosen = int.MinValue;
        obj.Close();
    }

    private void Ok(Window obj) => obj.Close();
}
