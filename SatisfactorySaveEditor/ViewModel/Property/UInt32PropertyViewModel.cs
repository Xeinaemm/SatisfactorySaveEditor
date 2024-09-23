using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class UInt32PropertyViewModel : SerializedPropertyViewModel
{
    private readonly UInt32Property model;

    [ObservableProperty]
    private uint value;

    public override string ShortName => "UInt32";

    public UInt32PropertyViewModel(UInt32Property uintProperty) : base(uintProperty)
    {
        model = uintProperty;
        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
