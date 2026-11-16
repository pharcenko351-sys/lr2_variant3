using System.Globalization;
using System.Text;
using System.Xml;
using DormitoryXml.Code.Models;

namespace DormitoryXml.Code.Strategies;

public class DomSearchStrategy : IXmlSearchStrategy
{
    public string Name => "DOM";

    public List<Student> Search(string path, SearchCriteria criteria)
    {
        var doc = new XmlDocument();
        doc.Load(path);

        string xpath = BuildXPath(criteria);
        var result = new List<Student>();

        foreach (XmlNode node in doc.SelectNodes(xpath)!)
        {
            var student = new Student
            {
                FullName = Attr(node, "FullName"),
                Faculty = Attr(node, "Faculty"),
                Department = Attr(node, "Department"),
                Course = int.Parse(Attr(node, "Course"), CultureInfo.InvariantCulture),
                Phone = node.Attributes?["Phone"]?.Value
            };

            foreach (XmlNode r in node.SelectNodes("residence")!)
            {
                string end = Attr(r, "EndDate");
                student.Residences.Add(new Residence
                {
                    Building = Attr(r, "Building"),
                    Room = Attr(r, "Room"),
                    StartDate = DateTime.Parse(Attr(r, "StartDate"), CultureInfo.InvariantCulture),
                    EndDate = end == "" ? null : DateTime.Parse(end, CultureInfo.InvariantCulture)
                });
            }

            if (criteria.Matches(student)) result.Add(student);
        }
        return result;
    }

    private static string BuildXPath(SearchCriteria c)
    {
        var sb = new StringBuilder("/dormitory/student");
        if (!string.IsNullOrWhiteSpace(c.Faculty)) sb.Append($"[@Faculty={Literal(c.Faculty.Trim())}]");
        if (!string.IsNullOrWhiteSpace(c.Department)) sb.Append($"[@Department={Literal(c.Department.Trim())}]");
        if (c.Course.HasValue) sb.Append($"[@Course={c.Course.Value}]");
        if (!string.IsNullOrWhiteSpace(c.Building)) sb.Append($"[residence/@Building={Literal(c.Building.Trim())}]");
        return sb.ToString();
    }

    private static string Literal(string s)
    {
        if (!s.Contains('\'')) return $"'{s}'";
        if (!s.Contains('"')) return $"\"{s}\"";
        return "concat('" + s.Replace("'", "', \"'\", '") + "')";
    }

    private static string Attr(XmlNode n, string name) => n.Attributes?[name]?.Value ?? "";
}
