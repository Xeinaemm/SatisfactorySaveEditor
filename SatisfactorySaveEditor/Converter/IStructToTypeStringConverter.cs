using System.Globalization;
using System.Windows.Data;
using SatisfactorySaveEditor.ViewModel.Property;
using SatisfactorySaveEditor.ViewModel.Struct;
using SatisfactorySaveParser.PropertyTypes.Structs;

namespace SatisfactorySaveEditor.Converter;

public class IStructToTypeStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            Box b => "Box",
            Color c => "Color (0 - 255)",
            DynamicStructDataViewModel dsd => "Dynamic struct data",
            InventoryItem ii => "Inventory item",
            LinearColor lc => "Linear color (0f - 1f)",
            Quat q => "Quaternion",
            RailroadTrackPosition rtp => "Railroad track position",
            Rotator r => "Rotator",
            Vector v => "Vector",
            Vector2D v2 => "Vector2D",
            Vector4D v4 => "Vector4",
            GuidStruct g => "Guid",
            FluidBox fb => "FluidBox",
            FINNetworkTrace nt => "FINNetworkTrace",
            SatisfactorySaveParser.PropertyTypes.Structs.DateTime dt => "DateTime",
            // TODO: This seems like a bad idea, but it works for now
            SerializedPropertyViewModel spvm => new SerializablePropertyToTypeStringConverter().Convert(spvm, targetType, parameter, culture),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
