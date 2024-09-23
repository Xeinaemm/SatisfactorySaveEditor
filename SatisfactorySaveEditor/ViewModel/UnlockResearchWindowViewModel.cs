using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;

namespace SatisfactorySaveEditor.ViewModel;

public partial class UnlockResearchWindowViewModel : ObservableObject
{
    public IRelayCommand OkCommand => new RelayCommand<Window>(Ok);
    public IRelayCommand CancelCommand => new RelayCommand<Window>(Cancel);
    public IRelayCommand AddOneCommand => new RelayCommand<IList>(AddOne, list => list?.Count > 0);
    public IRelayCommand RemoveOneCommand => new RelayCommand<IList>(RemoveOne, list => list?.Count > 0);
    public IRelayCommand AddAllCommand => new RelayCommand(AddAll, () => Available?.Count > 0);
    public IRelayCommand AddAllAlternativesCommand => new RelayCommand(AddAllAlternatives,
            () => Available?.Any(x => x.Contains(@"/Alternate/")) ?? false);
    public IRelayCommand RemoveAllCommand => new RelayCommand(RemoveAll, () => Unlocked?.Count > 0);

    [ObservableProperty]
    private ObservableCollection<string> available;

    [ObservableProperty]
    private ObservableCollection<string> unlocked;

    private static void Cancel(Window window)
    {
        window.DialogResult = false;
        window.Close();
    }

    private static void Ok(Window window)
    {
        window.DialogResult = true;
        window.Close();
    }

    private void AddOne(IList items)
    {
        if (items == null)
            return;

        foreach (var item in items.Cast<string>().Reverse().ToList())
        {
            _ = Available.Remove(item);
            Unlocked.Insert(0, item);
        }
    }

    private void RemoveOne(IList items)
    {
        if (items == null)
            return;

        foreach (var item in items.Cast<string>().Reverse().ToList())
        {
            _ = Unlocked.Remove(item);
            Available.Insert(0, item);
        }
    }

    private void AddAll()
    {
        foreach (var item in Available.Reverse().ToList())
        {
            Unlocked.Insert(0, item);
            _ = Available.Remove(item);
        }
    }

    private void AddAllAlternatives()
    {
        foreach (var item in Available.Where(x => x.Contains(@"/Alternate/")).Reverse().ToList())
        {
            Unlocked.Insert(0, item);
            _ = Available.Remove(item);
        }
    }

    private void RemoveAll()
    {
        foreach (var item in Unlocked.Reverse().ToList())
        {
            Available.Insert(0, item);
            _ = Unlocked.Remove(item);
        }
    }
}
