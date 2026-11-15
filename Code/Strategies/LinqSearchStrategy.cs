using System.Globalization;
using System.Xml.Linq;
using DormitoryXml.Code.Models;

namespace DormitoryXml.Code.Strategies;

public class LinqSearchStrategy : IXmlSearchStrategy
{
    public string Name => "LINQ to XML";

    public List<Student> Search(string path, SearchCriteria criteria)
    {
        var doc = XDocument.Load(path);

        IEnumerable<XElement> query = doc.Root!.Elements("student");

        if (!string.IsNullOrWhiteSpace(criteria.Faculty))
            query = query.Where(e => Same((string?)e.Attribute("Faculty"), criteria.Faculty));

        if (!string.IsNullOrWhiteSpace(criteria.Department))
            query = query.Where(e => Same((string?)e.Attribute("Department"), criteria.Department));

        if (criteria.Course.HasValue)
            query = query.Where(e => (int?)e.Attribute("Course") == criteria.Course.Value);

        if (!string.IsNullOrWhiteSpace(criteria.Building))
            query = query.Where(e => e.Elements("residence")
                                      .Any(r => Same((string?)r.Attribute("Building"), criteria.Building)));

        return query.Select(ToStudent).Where(criteria.Matches).ToList();
    }

    private static bool Same(string? a, string b) =>
        string.Equals(a, b.Trim(), StringComparison.OrdinalIgnoreCase);

    private static Student ToStudent(XElement e) => new()
    {
        FullName = (string?)e.Attribute("FullName") ?? "",
        Faculty = (string?)e.Attribute("Faculty") ?? "",
        Department = (string?)e.Attribute("Department") ?? "",
        Course = (int?)e.Attribute("Course") ?? 0,
        Phone = (string?)e.Attribute("Phone"),
        Residences = e.Elements("residence").Select(r => new Residence
        {
            Building = (string?)r.Attribute("Building") ?? "",
            Room = (string?)r.Attribute("Room") ?? "",
            StartDate = DateTime.Parse((string)r.Attribute("StartDate")!, CultureInfo.InvariantCulture),
            EndDate = ParseDate((string?)r.Attribute("EndDate"))
        }).ToList()
    };

    private static DateTime? ParseDate(string? s) =>
        string.IsNullOrEmpty(s) ? null : DateTime.Parse(s, CultureInfo.InvariantCulture);
}
