namespace DormitoryXml.Code.Models;

public class Residence
{
    public string Building { get; set; } = "";

    public string Room { get; set; } = "";

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public override string ToString()
    {
        string end = EndDate.HasValue ? EndDate.Value.ToString("dd.MM.yyyy") : "дотепер";
        return $"{Building}, кімн. {Room} ({StartDate:dd.MM.yyyy} – {end})";
    }
}
