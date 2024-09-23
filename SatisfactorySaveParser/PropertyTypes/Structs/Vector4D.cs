using SatisfactorySaveParser.Structures;

namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class Vector4D(BinaryReader reader) : IStructData
{
    public int SerializedLength => 16;
    public string Type => "Vector4";
    public Vector4 Data { get; set; } = reader.ReadVector4();

    public void Serialize(BinaryWriter writer, int buildVersion) => writer.Write(Data);
}
