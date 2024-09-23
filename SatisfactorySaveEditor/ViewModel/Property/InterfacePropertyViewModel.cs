using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class InterfacePropertyViewModel : SerializedPropertyViewModel
{
    private readonly InterfaceProperty model;

    [ObservableProperty]
    private string str1;

    [ObservableProperty]
    private string str2;

    public override string ShortName => "Interface";

    public InterfacePropertyViewModel(InterfaceProperty interfaceProperty) : base(interfaceProperty)
    {
        model = interfaceProperty;

        str1 = model.LevelName;
        str2 = model.PathName;
    }

    public override void ApplyChanges()
    {
        model.LevelName = Str1;
        model.PathName = Str2;
    }
}
