namespace SteadySchedule.Domain;

public class Shift
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime Date { get; set; }
    public string Position { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string SlotGroupId { get; set; } = Guid.NewGuid().ToString();
}