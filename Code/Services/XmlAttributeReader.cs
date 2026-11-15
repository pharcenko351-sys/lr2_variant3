using System.Xml;

namespace DormitoryXml.Code.Services;

public class XmlAttributeReader
{
    public Dictionary<string, List<string>> LoadValues(string path)
    {
        var sets = new Dictionary<string, SortedSet<string>>();
        var culture = StringComparer.Create(new System.Globalization.CultureInfo("uk-UA"), true);

        using var reader = XmlReader.Create(path);
        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element || !reader.HasAttributes) continue;

            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (!sets.TryGetValue(reader.Name, out var set))
                    sets[reader.Name] = set = new SortedSet<string>(culture);
                if (!string.IsNullOrWhiteSpace(reader.Value)) set.Add(reader.Value);
            }
            reader.MoveToElement();
        }
        return sets.ToDictionary(kv => kv.Key, kv => kv.Value.ToList());
    }
}
