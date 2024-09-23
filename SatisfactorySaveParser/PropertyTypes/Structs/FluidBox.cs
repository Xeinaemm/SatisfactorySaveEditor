namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class FluidBox(BinaryReader reader) : IStructData
{
    public float Unknown { get; set; } = reader.ReadSingle();

    public int SerializedLength => 4;
    public string Type => "FluidBox";

    public void Serialize(BinaryWriter writer, int buildVersion) => writer.Write(Unknown);
}
