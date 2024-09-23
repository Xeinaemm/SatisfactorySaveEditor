using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SatisfactorySaveParser.PropertyTypes.Structs;
using Color = System.Windows.Media.Color;

namespace SatisfactorySaveEditor.Converter;

public class LinearColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not LinearColor linearColor
            ? Brushes.Transparent
            : (object)new SolidColorBrush(Color.FromScRgb(linearColor.A, linearColor.R, linearColor.G, linearColor.B));

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
