using SatisfactorySaveParser.Structures;

namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class Vector(BinaryReader reader) : IStructData
{
    public int SerializedLength => 12;
    public string Type => "Vector";
    public Vector3 Data { get; set; } = reader.ReadVector3();

    public void Serialize(BinaryWriter writer, int buildVersion) => writer.Write(Data);
}
