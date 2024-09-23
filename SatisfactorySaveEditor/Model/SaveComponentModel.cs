using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactorySaveEditor.View;
using SatisfactorySaveEditor.ViewModel;
using SatisfactorySaveEditor.ViewModel.Property;
using SatisfactorySaveEditor.ViewModel.Struct;
using SatisfactorySaveParser;
using SatisfactorySaveParser.PropertyTypes.Structs;
using System.Windows;

namespace SatisfactorySaveEditor.Model;

public partial class SaveComponentModel : SaveObjectModel
{
    [ObservableProperty]
    private string parentEntityName;

    public RelayCommand FillInventoryCommand => new(FillInventory);

    public RelayCommand EmptyInventoryCommand => new(EmptyInventory);

    public SaveComponentModel(SaveComponent sc) : base(sc) => ParentEntityName = sc.ParentEntityName;

    public override void ApplyChanges()
    {
        base.ApplyChanges();

        var model = (SaveComponent)Model;

        model.ParentEntityName = ParentEntityName;
    }

    public ArrayPropertyViewModel Inventory => FindField<ArrayPropertyViewModel>("mInventoryStacks");

    public override bool MatchesFilter(string filter) => base.MatchesFilter(filter) || MatchesFilterInventory(filter);

    private bool MatchesFilterInventory(string filter) => Inventory?.Elements.Cast<StructPropertyViewModel>().Any(element =>
    {
        var structData = (DynamicStructDataViewModel)element.StructData;
        var item = (InventoryItem)((StructPropertyViewModel)structData.Fields[0]).StructData;
        return item.ItemType.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }) ?? false;

    private void FillInventory()
    {
        var dialog = new FillWindow();
        var fvm = (FillViewModel)dialog.DataContext;
        _ = dialog.ShowDialog();

        if (!fvm.IsConfirmed)
            return;
        var inv = Inventory;
        foreach (var element in inv.Elements.Cast<StructPropertyViewModel>())
        {
            var structData = (DynamicStructDataViewModel)element.StructData;
            var item = (InventoryItem)((StructPropertyViewModel)structData.Fields[0]).StructData;
            var numItems = (IntPropertyViewModel)structData.Fields[1];
            item.ItemType = fvm.SelectedItem.ItemPath;
            numItems.Value = fvm.SelectedItem.Quantity;
        }
        ApplyChanges();
    }

    private void EmptyInventory()
    {
        var inv = Inventory;
        foreach (var element in inv.Elements.Cast<StructPropertyViewModel>())
        {
            var structData = (DynamicStructDataViewModel)element.StructData;
            var item = (InventoryItem)((StructPropertyViewModel)structData.Fields[0]).StructData;
            var numItems = (IntPropertyViewModel)structData.Fields[1];
            item.ItemType = string.Empty;
            numItems.Value = 0;
        }
        ApplyChanges();
        _ = MessageBox.Show($"Inventory for storage {Title} emptied", "Inventory Emptied");
    }
}
