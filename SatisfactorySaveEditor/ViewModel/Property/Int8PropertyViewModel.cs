using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class Int8PropertyViewModel : SerializedPropertyViewModel
{
    private readonly Int8Property model;

    [ObservableProperty]
    private byte value;

    public override string ShortName => "Int8";

    public Int8PropertyViewModel(Int8Property intProperty) : base(intProperty)
    {
        model = intProperty;
        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
