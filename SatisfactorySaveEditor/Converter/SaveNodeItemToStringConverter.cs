using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using SatisfactorySaveEditor.Model;

namespace SatisfactorySaveEditor.Converter;

public class SaveNodeItemToStringConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
            return string.Empty;
        if (values[0] is not string title || values[1] is not ObservableCollection<SaveObjectModel> items)
            return string.Empty;

        var totalCount = Traverse(items, obj => obj.Items).Count(obj => obj.Items.Count == 0);
        var count = items.Count;
        string formatString;

        switch (count)
        {
            case 0:
                return $"{title}";
            case 1:
                formatString = $"{title} (1 entry, ";
                break;
            default:
                formatString = $"{title} ({count} entries, ";
                break;
        }

        return totalCount switch
        {
            1 => formatString + $"{totalCount} object)",
            _ => formatString + $"{totalCount} objects)",
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;

    public static IEnumerable<T> Traverse<T>(IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
    {
        var stack = new Stack<T>(items);

        while (stack.Count != 0)
        {
            var next = stack.Pop();
            yield return next;
            foreach (var child in childSelector(next))
                stack.Push(child);
        }
    }
}
