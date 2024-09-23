using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class ObjectPropertyViewModel : SerializedPropertyViewModel
{
    private readonly ObjectProperty model;

    [ObservableProperty]
    private string str1;

    [ObservableProperty]
    private string str2;

    public override string ShortName => "Object";

    public ObjectPropertyViewModel(ObjectProperty objectProperty) : base(objectProperty)
    {
        model = objectProperty;

        str1 = model.LevelName;
        str2 = model.PathName;
    }

    public override void ApplyChanges()
    {
        model.LevelName = Str1;
        model.PathName = Str2;
    }
}
