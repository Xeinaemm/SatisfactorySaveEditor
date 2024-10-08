﻿using SatisfactorySaveParser.Structures;
using System.Diagnostics;

namespace SatisfactorySaveParser.PropertyTypes;

public class ObjectProperty : SerializedProperty, IObjectReference
{
    public const string TypeName = nameof(ObjectProperty);
    public override string PropertyType => TypeName;
    public override int SerializedLength => LevelName.GetSerializedLength() + PathName.GetSerializedLength();

    public string LevelName { get; set; }
    public string PathName { get; set; }
    public SaveObject ReferencedObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ObjectProperty(string propertyName, string root = null, string name = null, int index = 0) : base(propertyName, index)
    {
        LevelName = root;
        PathName = name;
    }

    public ObjectProperty(string propertyName, int index) : base(propertyName, index)
    {
    }

    public override string ToString() => $"obj: {PathName}";

    public override void Serialize(BinaryWriter writer, int buildVersion, bool writeHeader = true)
    {
        base.Serialize(writer, buildVersion, writeHeader);

        writer.Write(SerializedLength);
        writer.Write(Index);
        writer.Write((byte)0);

        writer.WriteLengthPrefixedString(LevelName);
        writer.WriteLengthPrefixedString(PathName);
    }

    public static ObjectProperty Parse(string propertyName, int index, BinaryReader reader)
    {
        var result = new ObjectProperty(propertyName, index);

        var unk3 = reader.ReadByte();
        Trace.Assert(unk3 == 0);

        result.LevelName = reader.ReadLengthPrefixedString();
        result.PathName = reader.ReadLengthPrefixedString();

        return result;
    }
}
