﻿using System.Diagnostics;

namespace SatisfactorySaveParser.PropertyTypes;

public class Int64Property(string propertyName, int index = 0) : SerializedProperty(propertyName, index)
{
    public const string TypeName = nameof(Int64Property);
    public override string PropertyType => TypeName;

    public override int SerializedLength => 8;

    public long Value { get; set; }

    public override string ToString() => $"int64: {Value}";

    public override void Serialize(BinaryWriter writer, int buildVersion, bool writeHeader = true)
    {
        base.Serialize(writer, buildVersion, writeHeader);

        writer.Write(SerializedLength);
        writer.Write(Index);

        writer.Write((byte)0);
        writer.Write(Value);
    }

    public static Int64Property Parse(string propertyName, int index, BinaryReader reader)
    {
        var unk3 = reader.ReadByte();
        Trace.Assert(unk3 == 0);

        return new Int64Property(propertyName, index)
        {
            Value = reader.ReadInt64()
        };
    }
}
