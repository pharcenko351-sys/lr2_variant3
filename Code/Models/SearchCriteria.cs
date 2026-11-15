namespace DormitoryXml.Code.Models;

public class SearchCriteria
{
    public string Faculty { get; set; } = "";
    public string Department { get; set; } = "";
    public int? Course { get; set; }
    public string Building { get; set; } = "";

    public string Keyword { get; set; } = "";

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Faculty) && string.IsNullOrWhiteSpace(Department) &&
        Course is null && string.IsNullOrWhiteSpace(Building) && string.IsNullOrWhiteSpace(Keyword);

    public bool Matches(Student s)
    {
        if (!Eq(Faculty, s.Faculty)) return false;
        if (!Eq(Department, s.Department)) return false;
        if (Course.HasValue && s.Course != Course.Value) return false;

        if (!string.IsNullOrWhiteSpace(Building) &&
            !s.Residences.Any(r => string.Equals(r.Building, Building, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            var haystack = new List<string> { s.FullName, s.Faculty, s.Department, s.Course.ToString(), s.Phone ?? "" };
            foreach (var r in s.Residences) { haystack.Add(r.Building); haystack.Add(r.Room); }

            string kw = Keyword.Trim();
            if (!haystack.Any(h => h.Contains(kw, StringComparison.OrdinalIgnoreCase))) return false;
        }
        return true;
    }

    private static bool Eq(string criterion, string value) =>
        string.IsNullOrWhiteSpace(criterion) ||
        string.Equals(criterion.Trim(), value, StringComparison.OrdinalIgnoreCase);
}
