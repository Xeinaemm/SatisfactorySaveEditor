namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class DateTime(BinaryReader reader) : IStructData
{
    public long Timestamp { get; set; } = reader.ReadInt64();

    public int SerializedLength => 8;
    public string Type => "DateTime";

    public void Serialize(BinaryWriter writer, int buildVersion) => writer.Write(Timestamp);
}
