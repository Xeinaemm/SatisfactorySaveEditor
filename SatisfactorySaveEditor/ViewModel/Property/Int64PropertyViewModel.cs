using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class Int64PropertyViewModel : SerializedPropertyViewModel
{
    private readonly Int64Property model;

    [ObservableProperty]
    private long value;

    public override string ShortName => "Int64";

    public Int64PropertyViewModel(Int64Property intProperty) : base(intProperty)
    {
        model = intProperty;
        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
