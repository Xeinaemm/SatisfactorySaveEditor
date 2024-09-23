using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class BytePropertyViewModel : SerializedPropertyViewModel
{
    private readonly ByteProperty model;

    [ObservableProperty]
    private string type;

    [ObservableProperty]
    private string value;

    public override string ShortName => "Byte";

    public BytePropertyViewModel(ByteProperty byteProperty) : base(byteProperty)
    {
        model = byteProperty;

        value = model.Value;
        type = model.Type;
    }

    public override void ApplyChanges()
    {
        model.Value = Value;
        model.Type = Type;
    }
}
