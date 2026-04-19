namespace SteadySchedule.Domain;

public class Employee
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int MaxHoursPerWeek { get; set; }
/*public decimal? HourlyRate { get; set; }
public decimal? WeeklySalary { get; set; }
public bool IsSalary { get; set; }*/
    public string PositionsQualified { get; set; } = string.Empty; // v1 simple

    public bool MondayAvailable { get; set; }
    public TimeSpan? MondayStart { get; set; }
    public TimeSpan? MondayEnd { get; set; }

    public bool TuesdayAvailable { get; set; }
    public TimeSpan? TuesdayStart { get; set; }
    public TimeSpan? TuesdayEnd { get; set; }

    public bool WednesdayAvailable { get; set; }
    public TimeSpan? WednesdayStart { get; set; }
    public TimeSpan? WednesdayEnd { get; set; }

    public bool ThursdayAvailable { get; set; }
    public TimeSpan? ThursdayStart { get; set; }
    public TimeSpan? ThursdayEnd { get; set; }

    public bool FridayAvailable { get; set; }
    public TimeSpan? FridayStart { get; set; }
    public TimeSpan? FridayEnd { get; set; }

    public bool SaturdayAvailable { get; set; }
    public TimeSpan? SaturdayStart { get; set; }
    public TimeSpan? SaturdayEnd { get; set; }

    public bool SundayAvailable { get; set; }
    public TimeSpan? SundayStart { get; set; }
    public TimeSpan? SundayEnd { get; set; }

    public bool MondayAnyTime { get; set; }
    public bool TuesdayAnyTime { get; set; }
    public bool WednesdayAnyTime { get; set; }
    public bool ThursdayAnyTime { get; set; }
    public bool FridayAnyTime { get; set; }
    public bool SaturdayAnyTime { get; set; }
    public bool SundayAnyTime { get; set; }
}