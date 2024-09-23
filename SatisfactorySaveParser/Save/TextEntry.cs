﻿namespace SatisfactorySaveParser.Save;

public enum EFormatArgumentType
{
    Int,
    UInt,
    Float,
    Double,
    Text,
    Gender,
}

public enum ETextHistoryType
{
    None = 255,
    Base = 0,
    NamedFormat,
    OrderedFormat,
    ArgumentFormat,
    AsNumber,
    AsPercent,
    AsCurrency,
    AsDate,
    AsTime,
    AsDateTime,
    Transform,
    StringTableEntry,
    TextGenerator,
}

public abstract class TextEntry(int flags)
{
    public abstract ETextHistoryType HistoryType { get; }
    public abstract int SerializedLength { get; }

    public int Flags { get; set; } = flags;
}

public class BaseTextEntry(int flags) : TextEntry(flags), IEquatable<BaseTextEntry>
{
    public override ETextHistoryType HistoryType => ETextHistoryType.Base;
    public override int SerializedLength => 5 + Namespace.GetSerializedLength() + Key.GetSerializedLength() + Value.GetSerializedLength();

    public string Namespace { get; set; }

    /// <summary>
    ///     Unknown string value, possibly relating to string table
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    ///     Main text value
    /// </summary>
    public string Value { get; set; }

    public bool Equals(BaseTextEntry other) => other != null
&& Flags == other.Flags &&
            Namespace == other.Namespace &&
            Key == other.Key &&
            Value == other.Value;

    public override bool Equals(object obj) => Equals(obj as BaseTextEntry);

    public override int GetHashCode() => Flags + Namespace.GetHashCode() + (17 * Key.GetHashCode()) + (23 * Value.GetHashCode());
}

public class ArgumentFormat : IEquatable<ArgumentFormat>
{
    public int SerializedLength => Name.GetSerializedLength() + 1 + Value.SerializedLength;

    public string Name { get; set; }
    public EFormatArgumentType ValueType { get; set; }
    public TextEntry Value { get; set; }

    public bool Equals(ArgumentFormat other) => other != null
&& Name == other.Name &&
            Value.Equals(other.Value);

    public override bool Equals(object obj) => Equals(obj as ArgumentFormat);

    public override int GetHashCode() => Name.GetHashCode() + (17 * Value.GetHashCode());
}

public class ArgumentFormatTextEntry(int flags) : TextEntry(flags)
{
    public override ETextHistoryType HistoryType => ETextHistoryType.ArgumentFormat;
    public override int SerializedLength => 5 + SourceFormat.SerializedLength + 4 + Arguments.Sum(x => x.SerializedLength);

    public BaseTextEntry SourceFormat { get; set; }

    public List<ArgumentFormat> Arguments { get; } = [];
}

public class NoneTextEntry(int flags) : TextEntry(flags)
{
    public override ETextHistoryType HistoryType => ETextHistoryType.None;

    public override int SerializedLength
    {
        get
        {
            if (!HasCultureInvariantString.HasValue)
                return 5;

            return !HasCultureInvariantString.Value ? 9 : 9 + CultureInvariantString.GetSerializedLength();
        }
    }

    public bool? HasCultureInvariantString { get; set; } = null;
    public string CultureInvariantString { get; set; }
}

public class TransformTextEntry(int flags) : TextEntry(flags)
{
    public override ETextHistoryType HistoryType => ETextHistoryType.Transform;

    public override int SerializedLength => SourceText.SerializedLength + 1;

    public TextEntry SourceText { get; set; }
    public byte TransformType { get; set; }
}
