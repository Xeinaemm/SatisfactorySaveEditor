using SatisfactorySaveParser.Structures;

namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class Box(BinaryReader reader) : IStructData
{
    public Vector3 Min { get; set; } = reader.ReadVector3();
    public Vector3 Max { get; set; } = reader.ReadVector3();

    public byte UnknownByte { get; set; } = reader.ReadByte();

    public int SerializedLength => 25;
    public string Type => "Box";

    public void Serialize(BinaryWriter writer, int buildVersion)
    {
        writer.Write(Min);
        writer.Write(Max);
        writer.Write(UnknownByte);
    }
}
