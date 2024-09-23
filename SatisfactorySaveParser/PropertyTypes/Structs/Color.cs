namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class Color(BinaryReader reader) : IStructData
{
    public byte R { get; set; } = reader.ReadByte();
    public byte G { get; set; } = reader.ReadByte();
    public byte B { get; set; } = reader.ReadByte();
    public byte A { get; set; } = reader.ReadByte();

    public int SerializedLength => 4;
    public string Type => "Color";

    public void Serialize(BinaryWriter writer, int buildVersion)
    {
        writer.Write(B);
        writer.Write(G);
        writer.Write(R);
        writer.Write(A);
    }
}
