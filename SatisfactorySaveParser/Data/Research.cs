using System.Xml.Linq;

namespace SatisfactorySaveParser.Data;

public class Research(XElement element)
{
    public string Path { get; set; } = element.Attribute("value").Value;

    public static IEnumerable<Research> GetResearches()
    {
        var doc = XDocument.Load("Data/Research.xml");
        var node = doc.Element("ResearchData");

        return node.Elements("Research").Select(c => new Research(c));
    }
}
