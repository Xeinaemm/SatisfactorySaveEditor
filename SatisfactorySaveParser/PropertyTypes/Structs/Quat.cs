namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class Quat(BinaryReader reader) : IStructData
{
    public float X { get; set; } = reader.ReadSingle();
    public float Y { get; set; } = reader.ReadSingle();
    public float Z { get; set; } = reader.ReadSingle();
    public float W { get; set; } = reader.ReadSingle();

    public int SerializedLength => 16;
    public string Type => "Quat";

    public void Serialize(BinaryWriter writer, int buildVersion)
    {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
        writer.Write(W);
    }
}
