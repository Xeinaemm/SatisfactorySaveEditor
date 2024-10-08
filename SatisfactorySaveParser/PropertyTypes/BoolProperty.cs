﻿using System.Diagnostics;

namespace SatisfactorySaveParser.PropertyTypes;

public class BoolProperty(string propertyName, int index = 0) : SerializedProperty(propertyName, index)
{
    public const string TypeName = nameof(BoolProperty);
    public override string PropertyType => TypeName;
    public override int SerializedLength => 0;

    public bool Value { get; set; }

    public override string ToString() => $"bool: {Value}";

    public override void Serialize(BinaryWriter writer, int buildVersion, bool writeHeader = true)
    {
        base.Serialize(writer, buildVersion, writeHeader);

        writer.Write(SerializedLength);
        writer.Write(Index);

        writer.Write((byte)(Value ? 1 : 0));
        writer.Write((byte)0);
    }

    public static BoolProperty Parse(string propertyName, int index, BinaryReader reader)
    {
        var result = new BoolProperty(propertyName, index)
        {
            Value = reader.ReadByte() > 0
        };

        Trace.Assert(reader.ReadByte() == 0);

        return result;
    }
}
