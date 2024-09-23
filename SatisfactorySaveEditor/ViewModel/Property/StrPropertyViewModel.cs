using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class StrPropertyViewModel : SerializedPropertyViewModel
{
    private readonly StrProperty model;

    [ObservableProperty]
    private string value;

    public override string ShortName => "String";

    public StrPropertyViewModel(StrProperty strProperty) : base(strProperty)
    {
        model = strProperty;

        value = model.Value;
    }

    public override void ApplyChanges() => model.Value = Value;
}
