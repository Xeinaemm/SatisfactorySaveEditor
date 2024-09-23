namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class GuidStruct(BinaryReader reader) : IStructData
{
    public int SerializedLength => 16;

    public string Type => "Guid";

    public Guid Data { get; set; } = new Guid(reader.ReadBytes(16));

    public void Serialize(BinaryWriter writer, int buildVersion) => writer.Write(Data.ToByteArray());
}
