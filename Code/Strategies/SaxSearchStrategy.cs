using System.Globalization;
using System.Xml;
using DormitoryXml.Code.Models;

namespace DormitoryXml.Code.Strategies;

public class SaxSearchStrategy : IXmlSearchStrategy
{
    public string Name => "SAX (XmlReader)";

    public List<Student> Search(string path, SearchCriteria criteria)
    {
        var result = new List<Student>();
        Student? current = null;

        var settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
        using var reader = XmlReader.Create(path, settings);

        void Finish()
        {
            if (current != null && criteria.Matches(current)) result.Add(current);
            current = null;
        }

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == "student")
                {
                    current = new Student
                    {
                        FullName = reader.GetAttribute("FullName") ?? "",
                        Faculty = reader.GetAttribute("Faculty") ?? "",
                        Department = reader.GetAttribute("Department") ?? "",
                        Course = int.Parse(reader.GetAttribute("Course") ?? "0", CultureInfo.InvariantCulture),
                        Phone = reader.GetAttribute("Phone")
                    };
                    if (reader.IsEmptyElement) Finish();
                }
                else if (reader.Name == "residence" && current != null)
                {
                    string? end = reader.GetAttribute("EndDate");
                    current.Residences.Add(new Residence
                    {
                        Building = reader.GetAttribute("Building") ?? "",
                        Room = reader.GetAttribute("Room") ?? "",
                        StartDate = DateTime.Parse(reader.GetAttribute("StartDate")!, CultureInfo.InvariantCulture),
                        EndDate = string.IsNullOrEmpty(end) ? null : DateTime.Parse(end, CultureInfo.InvariantCulture)
                    });
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "student")
            {
                Finish();
            }
        }
        return result;
    }
}
