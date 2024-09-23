using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class NamePropertyViewModel : SerializedPropertyViewModel
{
    private readonly NameProperty model;

    [ObservableProperty]
    private string value;

    public override string ShortName => "Name";

    public NamePropertyViewModel(NameProperty nameProperty) : base(nameProperty)
    {
        model = nameProperty;
        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
