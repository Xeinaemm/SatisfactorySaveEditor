using SatisfactorySaveEditor.ViewModel.Property;
using SatisfactorySaveParser.PropertyTypes;

namespace SatisfactorySaveEditor.Util;

public static class PropertyViewModelMapper
{
    public static SerializedPropertyViewModel Convert(SerializedProperty property)
    {
        return property switch
        {
            ArrayProperty arp => new ArrayPropertyViewModel(arp),
            BoolProperty bop => new BoolPropertyViewModel(bop),
            ByteProperty byp => new BytePropertyViewModel(byp),
            EnumProperty enp => new EnumPropertyViewModel(enp),
            FloatProperty flp => new FloatPropertyViewModel(flp),
            DoubleProperty dlp => new DoublePropertyViewModel(dlp),
            IntProperty inp => new IntPropertyViewModel(inp),
            MapProperty map => new MapPropertyViewModel(map),
            NameProperty nap => new NamePropertyViewModel(nap),
            ObjectProperty obp => new ObjectPropertyViewModel(obp),
            StrProperty strip => new StrPropertyViewModel(strip),
            StructProperty strup => new StructPropertyViewModel(strup),
            TextProperty tep => new TextPropertyViewModel(tep),
            SetProperty sep => new SetPropertyViewModel(sep),
            InterfaceProperty ifp => new InterfacePropertyViewModel(ifp),
            Int64Property i64p => new Int64PropertyViewModel(i64p),
            UInt64Property ui64p => new UInt64PropertyViewModel(ui64p),
            UInt32Property ui32p => new UInt32PropertyViewModel(ui32p),
            Int8Property i8p => new Int8PropertyViewModel(i8p),
            _ => throw new ArgumentOutOfRangeException(nameof(property), property, null),
        };
    }

    public static SerializedPropertyViewModel Create<T>(string propertyName) where T : SerializedPropertyViewModel
    {
        if (typeof(T) == typeof(ArrayPropertyViewModel))
            return new ArrayPropertyViewModel(new ArrayProperty(propertyName));

        if (typeof(T) == typeof(BoolPropertyViewModel))
            return new BoolPropertyViewModel(new BoolProperty(propertyName));

        if (typeof(T) == typeof(BytePropertyViewModel))
            return new BytePropertyViewModel(new ByteProperty(propertyName));

        if (typeof(T) == typeof(EnumPropertyViewModel))
            return new EnumPropertyViewModel(new EnumProperty(propertyName));

        if (typeof(T) == typeof(FloatPropertyViewModel))
            return new FloatPropertyViewModel(new FloatProperty(propertyName));

        if (typeof(T) == typeof(DoublePropertyViewModel))
            return new DoublePropertyViewModel(new DoubleProperty(propertyName));

        if (typeof(T) == typeof(IntPropertyViewModel))
            return new IntPropertyViewModel(new IntProperty(propertyName));

        if (typeof(T) == typeof(MapPropertyViewModel))
            return new MapPropertyViewModel(new MapProperty(propertyName));

        if (typeof(T) == typeof(NamePropertyViewModel))
            return new NamePropertyViewModel(new NameProperty(propertyName));

        if (typeof(T) == typeof(ObjectPropertyViewModel))
            return new ObjectPropertyViewModel(new ObjectProperty(propertyName));

        if (typeof(T) == typeof(StrPropertyViewModel))
            return new StrPropertyViewModel(new StrProperty(propertyName));

        if (typeof(T) == typeof(StructPropertyViewModel))
            return new StructPropertyViewModel(new StructProperty(propertyName));

        if (typeof(T) == typeof(TextPropertyViewModel))
            return new TextPropertyViewModel(new TextProperty(propertyName));

        if (typeof(T) == typeof(SetPropertyViewModel))
            return new SetPropertyViewModel(new SetProperty(propertyName));

        if (typeof(T) == typeof(InterfacePropertyViewModel))
            return new InterfacePropertyViewModel(new InterfaceProperty(propertyName));

        if (typeof(T) == typeof(Int64PropertyViewModel))
            return new Int64PropertyViewModel(new Int64Property(propertyName));

        if (typeof(T) == typeof(UInt64PropertyViewModel))
            return new UInt64PropertyViewModel(new UInt64Property(propertyName));

        return typeof(T) == typeof(Int8PropertyViewModel)
            ? (SerializedPropertyViewModel)new Int8PropertyViewModel(new Int8Property(propertyName))
            : throw new NotImplementedException($"Can't instantiate unknown property type {typeof(T)}");
    }
}
