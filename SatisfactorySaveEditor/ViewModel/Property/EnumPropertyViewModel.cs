using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class EnumPropertyViewModel : SerializedPropertyViewModel
{
    private readonly EnumProperty model;

    [ObservableProperty]
    private string value;

    public override string ShortName => "Enum";

    public EnumPropertyViewModel(EnumProperty enumProperty) : base(enumProperty)
    {
        model = enumProperty;
        value = model.Name;
    }

    public override void ApplyChanges() => model.Name = Value;
}
