namespace SteadySchedule.Domain;

public class Employee
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int MaxHoursPerWeek { get; set; }
    public string PositionsQualified { get; set; } = string.Empty; // v1 simple
}