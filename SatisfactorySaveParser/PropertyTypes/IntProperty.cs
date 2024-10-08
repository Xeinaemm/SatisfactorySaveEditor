﻿using System.Diagnostics;

namespace SatisfactorySaveParser.PropertyTypes;

public class IntProperty(string propertyName, int index = 0) : SerializedProperty(propertyName, index)
{
    public const string TypeName = nameof(IntProperty);
    public override string PropertyType => TypeName;

    public override int SerializedLength => 4;

    public int Value { get; set; }

    public override string ToString() => $"int: {Value}";

    public override void Serialize(BinaryWriter writer, int buildVersion, bool writeHeader = true)
    {
        base.Serialize(writer, buildVersion, writeHeader);

        writer.Write(SerializedLength);
        writer.Write(Index);

        writer.Write((byte)0);
        writer.Write(Value);
    }

    public static IntProperty Parse(string propertyName, int index, BinaryReader reader)
    {
        var unk3 = reader.ReadByte();
        Trace.Assert(unk3 == 0);

        return new IntProperty(propertyName, index)
        {
            Value = reader.ReadInt32()
        };
    }
}
