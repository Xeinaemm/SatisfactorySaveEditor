namespace SatisfactorySaveParser.PropertyTypes.Structs;

public class Rotator(BinaryReader reader) : Vector(reader)
{
    public new static string Type => "Rotator";
}
