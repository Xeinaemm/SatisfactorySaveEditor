using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.PropertyTypes;
using SatisfactorySaveParser.Save;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class TextPropertyViewModel : SerializedPropertyViewModel
{
    private readonly TextProperty model;

    [ObservableProperty]
    private string value;

    public override string ShortName => "Text";

    public TextPropertyViewModel(TextProperty textProperty) : base(textProperty)
    {
        model = textProperty;

        switch (textProperty.Text)
        {
            case BaseTextEntry baseText:
                value = baseText.Value;
                break;
            case NoneTextEntry baseText:
                value = baseText.CultureInvariantString;
                break;
        }
    }

    public override void ApplyChanges()
    {
        switch (model.Text)
        {
            case BaseTextEntry baseText:
                baseText.Value = Value;
                break;
            case NoneTextEntry baseText:
                baseText.CultureInvariantString = Value;
                break;
        }
    }
}
