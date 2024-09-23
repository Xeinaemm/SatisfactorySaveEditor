using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactorySaveParser.PropertyTypes;
using System.Windows;

namespace SatisfactorySaveEditor.ViewModel.Property;

public abstract class SerializedPropertyViewModel(SerializedProperty serializedProperty) : ObservableObject
{
    public readonly SerializedProperty Model = serializedProperty;

    public string PropertyName => Model.PropertyName;

    public IRelayCommand CopyPropertyNameCommand => new RelayCommand(CopyPropertyName);

    /// <summary>
    /// Gets or sets the index of this property in an array
    /// Leave null for properties outside arrays
    /// </summary>
    public string Index { get; set; }

    public abstract string ShortName { get; }

    public abstract void ApplyChanges();

    private void CopyPropertyName() => Clipboard.SetText(PropertyName);
}
