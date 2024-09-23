namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class RailroadTrackPosition(BinaryReader reader) : IStructData
{
    public string Root { get; set; } = reader.ReadLengthPrefixedString();
    public string InstanceName { get; set; } = reader.ReadLengthPrefixedString();
    public float Offset { get; set; } = reader.ReadSingle();
    public float Forward { get; set; } = reader.ReadSingle();


    public int SerializedLength => Root.GetSerializedLength() + InstanceName.GetSerializedLength() + 8;
    public string Type => "RailroadTrackPosition";

    public void Serialize(BinaryWriter writer, int buildVersion)
    {
        writer.WriteLengthPrefixedString(Root);
        writer.WriteLengthPrefixedString(InstanceName);
        writer.Write(Offset);
        writer.Write(Forward);
    }
}
