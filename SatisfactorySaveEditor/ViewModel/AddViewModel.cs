using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactorySaveEditor.Model;
using SatisfactorySaveEditor.Util;
using SatisfactorySaveParser.PropertyTypes;
using System.Windows;

namespace SatisfactorySaveEditor.ViewModel;

public partial class AddViewModel : ObservableObject
{
    public enum AddTypeEnum
    {
        Array,
        Bool,
        Byte,
        Enum,
        Float,
        Int,
        Map,
        Name,
        Object,
        String,
        Struct,
        Text,
        Interface,
        Int64
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsArray))]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private AddTypeEnum type = AddTypeEnum.Array;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private AddTypeEnum arrayType = AddTypeEnum.Bool;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private string name;

    public SaveObjectModel ObjectModel { get; set; }
    public IRelayCommand OkCommand => new RelayCommand<Window>(Ok);
    public IRelayCommand CancelCommand => new RelayCommand<Window>(Cancel);

    public Visibility IsArray => Type == AddTypeEnum.Array ? Visibility.Visible : Visibility.Collapsed;

    public bool CanConfirm => Type != AddTypeEnum.Array
                ? !string.IsNullOrWhiteSpace(Name)
                : ArrayType != AddTypeEnum.Array && !string.IsNullOrWhiteSpace(Name);

    public static SerializedProperty CreateProperty(AddTypeEnum type, string name) => type switch
    {
        AddTypeEnum.Array => new ArrayProperty(name),
        AddTypeEnum.Bool => new BoolProperty(name),
        AddTypeEnum.Byte => new ByteProperty(name),
        AddTypeEnum.Enum => new EnumProperty(name),
        AddTypeEnum.Float => new FloatProperty(name),
        AddTypeEnum.Int => new IntProperty(name),
        AddTypeEnum.Map => new MapProperty(name),
        AddTypeEnum.Name => new NameProperty(name),
        AddTypeEnum.Object => new ObjectProperty(name),
        AddTypeEnum.String => new StrProperty(name),
        AddTypeEnum.Struct => new StructProperty(name),
        AddTypeEnum.Text => new TextProperty(name),
        AddTypeEnum.Interface => new InterfaceProperty(name),
        AddTypeEnum.Int64 => new Int64Property(name),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    public static string FromAddTypeEnum(AddTypeEnum type) => type switch
    {
        AddTypeEnum.Array => "ArrayProperty",
        AddTypeEnum.Bool => "BoolProperty",
        AddTypeEnum.Byte => "ByteProperty",
        AddTypeEnum.Enum => "EnumProperty",
        AddTypeEnum.Float => "FloatProperty",
        AddTypeEnum.Int => "IntProperty",
        AddTypeEnum.Map => "MapProperty",
        AddTypeEnum.Name => "NameProperty",
        AddTypeEnum.Object => "ObjectProperty",
        AddTypeEnum.String => "StrProperty",
        AddTypeEnum.Struct => "StructProperty",
        AddTypeEnum.Text => "TextProperty",
        AddTypeEnum.Interface => "InterfaceProperty",
        AddTypeEnum.Int64 => "Int64Property",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    public static AddTypeEnum FromStringType(string stringType) => stringType switch
    {
        "ArrayProperty" => AddTypeEnum.Array,
        "BoolProperty" => AddTypeEnum.Bool,
        "ByteProperty" => AddTypeEnum.Byte,
        "EnumProperty" => AddTypeEnum.Enum,
        "FloatProperty" => AddTypeEnum.Float,
        "IntProperty" => AddTypeEnum.Int,
        "MapProperty" => AddTypeEnum.Map,
        "NameProperty" => AddTypeEnum.Name,
        "ObjectProperty" => AddTypeEnum.Object,
        "StrProperty" => AddTypeEnum.String,
        "StructProperty" => AddTypeEnum.Struct,
        "TextProperty" => AddTypeEnum.Text,
        "InterfaceProperty" => AddTypeEnum.Interface,
        "Int64Property" => AddTypeEnum.Int64,
        _ => throw new ArgumentOutOfRangeException(nameof(stringType), stringType, null),
    };

    private void Cancel(Window obj) => obj.Close();

    private void Ok(Window obj)
    {
        var property = CreateProperty(Type, Name);
        if (Type == AddTypeEnum.Array)
            ((ArrayProperty)property).Type = FromAddTypeEnum(ArrayType);
        ObjectModel.Fields.Add(PropertyViewModelMapper.Convert(property));

        obj.Close();
    }
}
