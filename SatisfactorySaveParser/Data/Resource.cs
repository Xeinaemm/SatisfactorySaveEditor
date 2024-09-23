using System.Xml.Linq;

namespace SatisfactorySaveParser.Data;

public class Resource(XElement element)
{
    public string Path { get; set; } = element.Attribute("value").Value;
    public bool IsRadioactive { get; set; } = (element.Attribute("radioactive") != null) &&
        bool.Parse(element.Attribute("radioactive").Value);

    public static IEnumerable<Resource> GetResources()
    {
        var doc = XDocument.Load("Data/Resource.xml");
        var node = doc.Element("ResourceData");

        return node.Elements("Resource").Select(c => new Resource(c));
    }
}