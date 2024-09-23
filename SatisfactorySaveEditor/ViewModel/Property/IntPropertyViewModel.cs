using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class IntPropertyViewModel : SerializedPropertyViewModel
{
    private readonly IntProperty model;

    [ObservableProperty]
    private int value;

    public override string ShortName => "Int";

    public IntPropertyViewModel(IntProperty intProperty) : base(intProperty)
    {
        model = intProperty;
        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
