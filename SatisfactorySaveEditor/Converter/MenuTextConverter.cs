using System.Globalization;
using System.Windows.Data;

namespace SatisfactorySaveEditor.Converter;

public class MenuTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString().Replace("_", "__") ?? string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
