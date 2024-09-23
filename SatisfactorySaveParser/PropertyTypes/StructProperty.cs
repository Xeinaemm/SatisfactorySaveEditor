using SatisfactorySaveParser.PropertyTypes.Structs;
using System.Diagnostics;

namespace SatisfactorySaveParser.PropertyTypes;

public class StructProperty(string propertyName, int index = 0) : SerializedProperty(propertyName, index)
{
    public const string TypeName = nameof(StructProperty);
    public override string PropertyType => TypeName;
    public override int SerializedLength => Data.SerializedLength;

    public string Type => Data.Type;
    public int Unk1 { get; set; }
    public int Unk2 { get; set; }
    public int Unk3 { get; set; }
    public int Unk4 { get; set; }
    public byte Unk5 { get; set; }

    public IStructData Data { get; set; }

    public override string ToString() => $"struct {Type}";

    public override void Serialize(BinaryWriter writer, int buildVersion, bool writeHeader = true)
    {
        base.Serialize(writer, buildVersion, writeHeader);

        using (var ms = new MemoryStream())
        using (var msWriter = new BinaryWriter(ms))
        {
            Data.Serialize(msWriter, buildVersion);

            var bytes = ms.ToArray();

            writer.Write(bytes.Length);
            writer.Write(Index);

            writer.WriteLengthPrefixedString(Type);
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Write(Unk3);
            writer.Write(Unk4);
            writer.Write(Unk5);
            writer.Write(bytes);
        }            
    }

    public static int GetSerializedArrayLength(StructProperty[] properties)
    {
        var size = 4;

        var first = properties[0];

        size += first.PropertyName.GetSerializedLength();
        size += TypeName.GetSerializedLength();

        size += 8;

        size += first.Data.Type.GetSerializedLength();

        size += 17;

        size += properties.Sum(p => p.Data.SerializedLength);

        return size;
    }

    public static void SerializeArray(BinaryWriter writer, StructProperty[] properties, int buildVersion)
    {
        writer.Write(properties.Length);

        var first = properties[0];
        writer.WriteLengthPrefixedString(first.PropertyName);

        writer.WriteLengthPrefixedString(TypeName);

        using (var ms = new MemoryStream())
        using (var msWriter = new BinaryWriter(ms))
        {
            for (var i = 0; i < properties.Length; i++)
            {
                properties[i].Data.Serialize(msWriter, buildVersion);
            }

            var bytes = ms.ToArray();

            writer.Write(bytes.Length);

            writer.Write(first.Index);

            writer.WriteLengthPrefixedString(first.Data.Type);

            writer.Write(first.Unk1);
            writer.Write(first.Unk2);
            writer.Write(first.Unk3);
            writer.Write(first.Unk4);
            writer.Write(first.Unk5);

            writer.Write(bytes);
        }
    }

    private static IStructData ParseStructData(BinaryReader reader, string type, int size, int buildVersion)
    {
        return type switch
        {
            "LinearColor" => new LinearColor(reader),
            "Color" => new Color(reader),
            "Rotator" => new Rotator(reader),
            "Vector" => new Vector(reader),
            "Vector2D" => new Vector2D(reader),
            "Vector4" => new Vector4D(reader),
            "Box" => new Box(reader),
            "Quat" => new Quat(reader),
            "InventoryItem" => new InventoryItem(reader),
            "RailroadTrackPosition" => new RailroadTrackPosition(reader),
            "Guid" => new GuidStruct(reader),
            "FluidBox" => new FluidBox(reader),
            "FINNetworkTrace" => new FINNetworkTrace(reader),
            "FINLuaProcessorStateStorage" => new FINLuaProcessorStateStorage(reader, size),
            "DateTime" => new Structs.DateTime(reader),
            /*
case "InventoryStack":
case "InventoryItem":
case "PhaseCost":
case "ItemAmount":
case "ResearchCost":
case "CompletedResearch":
case "ResearchRecipeReward":
case "ItemFoundData":
case "RecipeAmountStruct":
case "MessageData":
case "SplinePointData":
return new DynamicStructData(reader);
*/
            _ => new DynamicStructData(reader, type, buildVersion),
        };
    }

    public static StructProperty[] ParseArray(BinaryReader reader, int buildVersion)
    {
        var count = reader.ReadInt32();
        var result = new StructProperty[count];

        var name = reader.ReadLengthPrefixedString();

        var propertyType = reader.ReadLengthPrefixedString();
        Trace.Assert(propertyType == "StructProperty");

        var size = reader.ReadInt32();
        var index = reader.ReadInt32();

        var structType = reader.ReadLengthPrefixedString();


        var unk1 = reader.ReadInt32();
        //Trace.Assert(unk1 == 0);

        var unk2 = reader.ReadInt32();
        //Trace.Assert(unk2 == 0);

        var unk3 = reader.ReadInt32();
        //Trace.Assert(unk3 == 0);

        var unk4 = reader.ReadInt32();
        //Trace.Assert(unk4 == 0);

        var unk5 = reader.ReadByte();
        Trace.Assert(unk5 == 0);

        for(var i = 0; i < count; i++)
        {
            result[i] = new StructProperty(name, index)
            {
                Unk1 = unk1,
                Unk2 = unk2,
                Unk3 = unk3,
                Unk4 = unk4,
                Unk5 = unk5,
                Data = ParseStructData(reader, structType, size, buildVersion)
            };
        }


        return result;
    }

    public static StructProperty Parse(string propertyName, int index, BinaryReader reader, int size, out int overhead, int buildVersion)
    {
        var result = new StructProperty(propertyName, index);
        var type = reader.ReadLengthPrefixedString();

        overhead = type.Length + 22;

        // StructGuid
        result.Unk1 = reader.ReadInt32();
        //Trace.Assert(result.Unk1 == 0);

        result.Unk2 = reader.ReadInt32();
        //Trace.Assert(result.Unk2 == 0);

        result.Unk3 = reader.ReadInt32();
        //Trace.Assert(result.Unk3 == 0);

        result.Unk4 = reader.ReadInt32();
        //Trace.Assert(result.Unk4 == 0);

        result.Unk5 = reader.ReadByte();
        Trace.Assert(result.Unk5 == 0);

        var before = reader.BaseStream.Position;
        result.Data = ParseStructData(reader, type, size, buildVersion);
        var after = reader.BaseStream.Position;

        return before + size != after ? throw new InvalidOperationException($"Expected {size} bytes read but got {after - before}") : result;
    }
}
