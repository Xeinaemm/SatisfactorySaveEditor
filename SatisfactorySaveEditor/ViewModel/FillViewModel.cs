using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactorySaveEditor.Model;

namespace SatisfactorySaveEditor.ViewModel;

public partial class FillViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private ResourceType selectedItem;

    public bool IsConfirmed { get; set; }

    public static List<ResourceType> ItemTypes => ResourceTypes.RESOURCES;

    public IRelayCommand OkCommand => new RelayCommand<Window>(Confirmed);

    public IRelayCommand CancelCommand => new RelayCommand<Window>(Cancelled);

    private void Confirmed(Window window)
    {
        IsConfirmed = true;
        window?.Close();
    }

    private void Cancelled(Window window) => window?.Close();
    public bool CanConfirm => !string.IsNullOrEmpty(SelectedItem.ItemPath) && SelectedItem.Quantity > 0;
}