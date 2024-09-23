using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveEditor.Util;
using SatisfactorySaveEditor.ViewModel.Property;
using SatisfactorySaveParser.PropertyTypes.Structs;

namespace SatisfactorySaveEditor.ViewModel.Struct;

public class DynamicStructDataViewModel(DynamicStructData dynamicStruct) : ObservableObject
{
    private readonly DynamicStructData model = dynamicStruct;

    public ObservableCollection<SerializedPropertyViewModel> Fields { get; }
        = new ObservableCollection<SerializedPropertyViewModel>(dynamicStruct.Fields.Select(PropertyViewModelMapper.Convert));

    public void ApplyChanges()
    {
        model.Fields.Clear();
        foreach (var field in Fields)
        {
            field.ApplyChanges();
            model.Fields.Add(field.Model);
        }
    }
}
