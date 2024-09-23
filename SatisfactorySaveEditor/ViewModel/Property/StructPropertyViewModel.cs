using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveEditor.ViewModel.Struct;
using SatisfactorySaveParser.PropertyTypes;
using SatisfactorySaveParser.PropertyTypes.Structs;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class StructPropertyViewModel : SerializedPropertyViewModel
{
    private readonly StructProperty model;

    [ObservableProperty]
    private object structData;

    public string Type => model.Type;

    public override string ShortName => $"Struct ({Type})";

    public StructPropertyViewModel(StructProperty structProperty) : base(structProperty)
    {
        model = structProperty;
        structData = model.Data is DynamicStructData dsd ? new DynamicStructDataViewModel(dsd) : (object)model.Data;
    }

    public override void ApplyChanges()
    {
        if (StructData is DynamicStructDataViewModel dsdvm)
            dsdvm.ApplyChanges();
        else
            model.Data = (IStructData)StructData;
    }
}
