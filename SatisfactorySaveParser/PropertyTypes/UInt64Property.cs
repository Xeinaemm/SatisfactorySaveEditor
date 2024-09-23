﻿using System.Diagnostics;

namespace SatisfactorySaveParser.PropertyTypes;

public class UInt64Property(string propertyName, int index = 0) : SerializedProperty(propertyName, index)
{
    public const string TypeName = nameof(UInt64Property);
    public override string PropertyType => TypeName;

    public override int SerializedLength => 8;

    public ulong Value { get; set; }

    public override string ToString() => $"uint64: {Value}";

    public override void Serialize(BinaryWriter writer, int buildVersion, bool writeHeader = true)
    {
        base.Serialize(writer, buildVersion, writeHeader);

        writer.Write(SerializedLength);
        writer.Write(Index);

        writer.Write((byte)0);
        writer.Write(Value);
    }

    public static UInt64Property Parse(string propertyName, int index, BinaryReader reader)
    {
        var unk3 = reader.ReadByte();
        Trace.Assert(unk3 == 0);

        return new UInt64Property(propertyName, index)
        {
            Value = reader.ReadUInt64()
        };
    }
}
