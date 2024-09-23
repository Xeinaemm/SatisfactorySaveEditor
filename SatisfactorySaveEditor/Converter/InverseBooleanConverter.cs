using System.Globalization;
using System.Windows.Data;

namespace SatisfactorySaveEditor.Converter;

//based off of https://stackoverflow.com/questions/1039636/how-to-bind-inverse-boolean-properties-in-wpf
[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture) => targetType != typeof(bool) ? throw new InvalidOperationException("The target must be a boolean") : (object)!(bool)value;

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture) => throw new NotSupportedException();
}
