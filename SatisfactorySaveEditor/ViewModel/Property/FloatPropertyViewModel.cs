using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class FloatPropertyViewModel : SerializedPropertyViewModel
{
    private readonly FloatProperty model;

    [ObservableProperty]
    private float value;

    public override string ShortName => "Float";

    public FloatPropertyViewModel(FloatProperty floatProperty) : base(floatProperty)
    {
        model = floatProperty;
        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
