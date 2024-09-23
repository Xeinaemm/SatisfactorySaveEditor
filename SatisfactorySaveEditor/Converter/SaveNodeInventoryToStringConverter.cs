using System.Globalization;
using System.Windows.Data;
using SatisfactorySaveEditor.Model;
using SatisfactorySaveEditor.ViewModel.Property;
using SatisfactorySaveEditor.ViewModel.Struct;
using SatisfactorySaveParser.PropertyTypes.Structs;

namespace SatisfactorySaveEditor.Converter;

public class SaveNodeInventoryToStringConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 1 || values[0] is not ArrayPropertyViewModel inv)
            return string.Empty;

        if (inv.Elements.Count == 0)
            return string.Empty;
        
        var (singlarStoredType, total) = getSinglarStoredType(inv);
        if (singlarStoredType == string.Empty)
            return string.Empty;

        var itemInfo = ResourceTypes.RESOURCES.Find(x => x.ItemPath == singlarStoredType);
        return itemInfo == null || itemInfo.Name == string.Empty ? string.Empty : (object)$"[{total} {itemInfo.Name}]";
    }

    private static (string, int) getSinglarStoredType(ArrayPropertyViewModel inv)
    {
        var foundItemType = string.Empty;
        var total = 0;
        foreach (StructPropertyViewModel element in inv.Elements)
        {
            var structData = (DynamicStructDataViewModel)element.StructData;
            var item = (InventoryItem)((StructPropertyViewModel)structData.Fields[0]).StructData;
            var numItems = (IntPropertyViewModel)structData.Fields[1];
            if (item.ItemType != string.Empty && foundItemType != item.ItemType)
            {
                if (foundItemType == string.Empty)
                {
                    foundItemType = item.ItemType;
                    total = numItems.Value;
                } else
                {
                    // there is more than one type in this inventory
                    return (string.Empty, 0);
                }
            } 
            else
            {
                total += numItems.Value;
            }
        }

        return (foundItemType, total);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
}
