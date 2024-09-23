using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class BoolPropertyViewModel : SerializedPropertyViewModel
{
    private readonly BoolProperty model;

    [ObservableProperty]
    private bool value;

    public override string ShortName => "Boolean";

    public BoolPropertyViewModel(BoolProperty boolProperty) : base(boolProperty)
    {
        model = boolProperty;

        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
