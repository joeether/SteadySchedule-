using SteadySchedule.Domain;

namespace SteadySchedule.Services;


public class ScheduleService
{

    public Dictionary<DateTime, WeekStatus> WeekStatuses { get; } = new();
    public Company Company { get; } = new()
    {
        Id = 1,
        Name = "Mama's Burger Joint",
        AdminEmail = "admin@mamasburgers.com"
    };

    public List<string> Positions { get; } = new()
    {
        "Cashier",
        "Cook"
    };

    public List<Employee> Employees { get; } = new();

    public static DateTime StartOfWeek(DateTime date)
    {
        // Monday-based
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.Date.AddDays(-diff);
    }

    public WeekStatus GetWeekStatus(DateTime weekStart)
    {
        weekStart = StartOfWeek(weekStart);

        if (WeekStatuses.TryGetValue(weekStart, out var status))
            return status;

        return WeekStatus.Draft;
    }

    public void PublishWeek(DateTime weekStart)
    {
        weekStart = StartOfWeek(weekStart);
        WeekStatuses[weekStart] = WeekStatus.Published;
    }

    public void UnpublishWeek(DateTime weekStart)
    {
        weekStart = StartOfWeek(weekStart);

        // If it was ever published, unpublishing means "InReview"
        var current = GetWeekStatus(weekStart);
        WeekStatuses[weekStart] = current == WeekStatus.Draft
            ? WeekStatus.Draft
            : WeekStatus.InReview;
    }

    public void SetWeekDraft(DateTime weekStart)
    {
        weekStart = StartOfWeek(weekStart);
        WeekStatuses[weekStart] = WeekStatus.Draft;
    }

    public IEnumerable<DateTime> GetPublishedWeeks()
    {
        return WeekStatuses
            .Where(kvp => kvp.Value == WeekStatus.Published || kvp.Value == WeekStatus.InReview)
            .Select(kvp => kvp.Key)
            .OrderByDescending(d => d);
    }

    public List<Shift> Shifts { get; } = new()
    {
        new Shift { Id = 1, CompanyId = 1, Date = DateTime.Today, Position="Cashier", StartTime=new TimeSpan(9,0,0), EndTime=new TimeSpan(13,0,0) },
        new Shift { Id = 2, CompanyId = 1, Date = DateTime.Today, Position="Cook", StartTime=new TimeSpan(10,0,0), EndTime=new TimeSpan(18,0,0) },
        new Shift { Id = 3, CompanyId = 1, Date = DateTime.Today, Position="Cashier", StartTime=new TimeSpan(12,0,0), EndTime=new TimeSpan(20,0,0) },
        new Shift { Id = 4, CompanyId = 1, Date = DateTime.Today.AddDays(1), Position="Cashier", StartTime=new TimeSpan(12,0,0), EndTime=new TimeSpan(20,0,0) },
    };

    public List<Assignment> Assignments { get; } = new();
}