using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class UInt64PropertyViewModel : SerializedPropertyViewModel
{
    private readonly UInt64Property model;

    [ObservableProperty]
    private ulong value;

    public override string ShortName => "UInt64";

    public UInt64PropertyViewModel(UInt64Property uintProperty) : base(uintProperty)
    {
        model = uintProperty;
        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
