using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class DoublePropertyViewModel : SerializedPropertyViewModel
{
    private readonly DoubleProperty model;

    [ObservableProperty]
    private double value;

    public override string ShortName => "Double";

    public DoublePropertyViewModel(DoubleProperty doubleProperty) : base(doubleProperty)
    {
        model = doubleProperty;
        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
