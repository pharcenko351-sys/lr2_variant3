using System.Globalization;
using System.Xml.Linq;
using DormitoryXml.Code.Models;

namespace DormitoryXml.Code.Services;

public class ResultsXmlBuilder
{
    public void Build(IEnumerable<Student> students, string path)
    {
        var root = new XElement("dormitory");
        foreach (var s in students)
        {
            var student = new XElement("student",
                new XAttribute("FullName", s.FullName),
                new XAttribute("Faculty", s.Faculty),
                new XAttribute("Department", s.Department),
                new XAttribute("Course", s.Course.ToString(CultureInfo.InvariantCulture)));
            if (!string.IsNullOrWhiteSpace(s.Phone)) student.Add(new XAttribute("Phone", s.Phone));

            foreach (var r in s.Residences)
            {
                var res = new XElement("residence",
                    new XAttribute("Building", r.Building),
                    new XAttribute("Room", r.Room),
                    new XAttribute("StartDate", r.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                if (r.EndDate.HasValue)
                    res.Add(new XAttribute("EndDate", r.EndDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                student.Add(res);
            }
            root.Add(student);
        }
        new XDocument(new XDeclaration("1.0", "utf-8", null), root).Save(path);
    }
}
