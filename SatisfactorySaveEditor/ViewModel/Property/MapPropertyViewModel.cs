using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.ViewModel.Property;

public class MapPropertyViewModel(MapProperty mapProperty) : SerializedPropertyViewModel(mapProperty)
{
    public override string ShortName => "Map";

    public override void ApplyChanges()
    {
    }
}
