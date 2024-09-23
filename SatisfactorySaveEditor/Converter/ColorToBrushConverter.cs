using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace SatisfactorySaveEditor.Converter;

public class ColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not SatisfactorySaveParser.PropertyTypes.Structs.Color color
            ? Brushes.Transparent
            : (object)new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
