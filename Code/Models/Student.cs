namespace DormitoryXml.Code.Models;

public class Student
{
    public string FullName { get; set; } = "";
    public string Faculty { get; set; } = "";
    public string Department { get; set; } = "";
    public int Course { get; set; }

    public string? Phone { get; set; }

    public List<Residence> Residences { get; set; } = new();


    public string Subtitle => $"{Faculty} · {Department} · {Course} курс";

    public string PhoneText => string.IsNullOrWhiteSpace(Phone) ? "тел.: —" : $"тел.: {Phone}";

    public string ResidencesText => string.Join(Environment.NewLine, Residences.Select(r => "🏠 " + r));
}
