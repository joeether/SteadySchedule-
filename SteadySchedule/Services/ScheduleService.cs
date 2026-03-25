using SteadySchedule.Domain;

namespace SteadySchedule.Services;


public class ScheduleService
{

    public Dictionary<DateTime, WeekStatus> WeekStatuses { get; } = new();

    private static readonly DateTime SeedWeekStart = StartOfWeek(DateTime.Today);

    public ScheduleService()
    {
        WeekStatuses[SeedWeekStart] = WeekStatus.Published;
    }
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

    public List<Employee> Employees { get; } = new()
{
    new Employee
    {
        Id = 1,
        CompanyId = 1,
        Name = "Amy",
        MaxHoursPerWeek = 40,
        PositionsQualified = "Cashier",
        MondayAvailable = true, MondayStart = new TimeSpan(9,0,0), MondayEnd = new TimeSpan(17,0,0),
        TuesdayAvailable = true, TuesdayStart = new TimeSpan(9,0,0), TuesdayEnd = new TimeSpan(17,0,0),
        WednesdayAvailable = true, WednesdayStart = new TimeSpan(9,0,0), WednesdayEnd = new TimeSpan(17,0,0),
        ThursdayAvailable = true, ThursdayStart = new TimeSpan(9,0,0), ThursdayEnd = new TimeSpan(17,0,0),
        FridayAvailable = true, FridayStart = new TimeSpan(9,0,0), FridayEnd = new TimeSpan(17,0,0)
    },
    new Employee
    {
        Id = 2,
        CompanyId = 1,
        Name = "Brandon",
        MaxHoursPerWeek = 40,
        PositionsQualified = "Cook",
        MondayAvailable = true, MondayStart = new TimeSpan(10,0,0), MondayEnd = new TimeSpan(18,0,0),
        TuesdayAvailable = true, TuesdayStart = new TimeSpan(10,0,0), TuesdayEnd = new TimeSpan(18,0,0),
        WednesdayAvailable = true, WednesdayStart = new TimeSpan(10,0,0), WednesdayEnd = new TimeSpan(18,0,0),
        ThursdayAvailable = true, ThursdayStart = new TimeSpan(10,0,0), ThursdayEnd = new TimeSpan(18,0,0),
        FridayAvailable = true, FridayStart = new TimeSpan(10,0,0), FridayEnd = new TimeSpan(18,0,0)
    },
    new Employee
    {
        Id = 3,
        CompanyId = 1,
        Name = "Carla",
        MaxHoursPerWeek = 24,
        PositionsQualified = "Cashier",
        SaturdayAvailable = true, SaturdayStart = new TimeSpan(10,0,0), SaturdayEnd = new TimeSpan(16,0,0),
        SundayAvailable = true, SundayStart = new TimeSpan(10,0,0), SundayEnd = new TimeSpan(16,0,0)
    },
    new Employee
    {
        Id = 4,
        CompanyId = 1,
        Name = "Derek",
        MaxHoursPerWeek = 32,
        PositionsQualified = "Cook,Cashier",
        WednesdayAvailable = true, WednesdayStart = new TimeSpan(10,0,0), WednesdayEnd = new TimeSpan(20,0,0),
        ThursdayAvailable = true, ThursdayStart = new TimeSpan(10,0,0), ThursdayEnd = new TimeSpan(20,0,0),
        FridayAvailable = true, FridayStart = new TimeSpan(10,0,0), FridayEnd = new TimeSpan(20,0,0),
        SaturdayAvailable = true, SaturdayStart = new TimeSpan(10,0,0), SaturdayEnd = new TimeSpan(20,0,0),
        SundayAvailable = true, SundayStart = new TimeSpan(10,0,0), SundayEnd = new TimeSpan(20,0,0)
    },
    new Employee
    {
        Id = 5,
        CompanyId = 1,
        Name = "Emma",
        MaxHoursPerWeek = 20,
        PositionsQualified = "Cashier",
        MondayAvailable = true, MondayStart = new TimeSpan(9,0,0), MondayEnd = new TimeSpan(13,0,0),
        WednesdayAvailable = true, WednesdayStart = new TimeSpan(9,0,0), WednesdayEnd = new TimeSpan(13,0,0),
        FridayAvailable = true, FridayStart = new TimeSpan(9,0,0), FridayEnd = new TimeSpan(13,0,0)
    }
};

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

    public List<WeekTemplate> WeekTemplates { get; set; } = new();
    public List<WeekTemplateShift> WeekTemplateShifts { get; set; } = new();

    public List<Shift> Shifts { get; } = new()
{
    new Shift { Id = 1,  CompanyId = 1, Date = SeedWeekStart.AddDays(0), Position = "Cashier", StartTime = new TimeSpan(9,0,0),  EndTime = new TimeSpan(13,0,0) },
    new Shift { Id = 2,  CompanyId = 1, Date = SeedWeekStart.AddDays(0), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

    new Shift { Id = 3,  CompanyId = 1, Date = SeedWeekStart.AddDays(1), Position = "Cashier", StartTime = new TimeSpan(9,0,0),  EndTime = new TimeSpan(13,0,0) },
    new Shift { Id = 4,  CompanyId = 1, Date = SeedWeekStart.AddDays(1), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

    new Shift { Id = 5,  CompanyId = 1, Date = SeedWeekStart.AddDays(2), Position = "Cashier", StartTime = new TimeSpan(9,0,0),  EndTime = new TimeSpan(13,0,0) },
    new Shift { Id = 6,  CompanyId = 1, Date = SeedWeekStart.AddDays(2), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

    new Shift { Id = 7,  CompanyId = 1, Date = SeedWeekStart.AddDays(3), Position = "Cashier", StartTime = new TimeSpan(9,0,0),  EndTime = new TimeSpan(13,0,0) },
    new Shift { Id = 8,  CompanyId = 1, Date = SeedWeekStart.AddDays(3), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

    new Shift { Id = 9,  CompanyId = 1, Date = SeedWeekStart.AddDays(4), Position = "Cashier", StartTime = new TimeSpan(9,0,0),  EndTime = new TimeSpan(13,0,0) },
    new Shift { Id = 10, CompanyId = 1, Date = SeedWeekStart.AddDays(4), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

    new Shift { Id = 11, CompanyId = 1, Date = SeedWeekStart.AddDays(5), Position = "Cashier", StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(14,0,0) },
    new Shift { Id = 12, CompanyId = 1, Date = SeedWeekStart.AddDays(5), Position = "Cook",    StartTime = new TimeSpan(11,0,0), EndTime = new TimeSpan(19,0,0) },

    new Shift { Id = 13, CompanyId = 1, Date = SeedWeekStart.AddDays(6), Position = "Cashier", StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(14,0,0) },
    new Shift { Id = 14, CompanyId = 1, Date = SeedWeekStart.AddDays(6), Position = "Cook",    StartTime = new TimeSpan(11,0,0), EndTime = new TimeSpan(19,0,0) }
};

    public List<Assignment> Assignments { get; } = new()
{
    new Assignment { Id = 1, ShiftId = 1,  EmployeeId = 1, ApprovedOvertime = false },
    new Assignment { Id = 2, ShiftId = 2,  EmployeeId = 2, ApprovedOvertime = false },
    new Assignment { Id = 3, ShiftId = 3,  EmployeeId = 1, ApprovedOvertime = false },
    new Assignment { Id = 4, ShiftId = 4,  EmployeeId = 2, ApprovedOvertime = false },
    new Assignment { Id = 5, ShiftId = 5,  EmployeeId = 5, ApprovedOvertime = false },
    new Assignment { Id = 6, ShiftId = 6,  EmployeeId = 4, ApprovedOvertime = false },
    new Assignment { Id = 7, ShiftId = 7,  EmployeeId = 1, ApprovedOvertime = false },
    new Assignment { Id = 8, ShiftId = 8,  EmployeeId = 4, ApprovedOvertime = false },
    new Assignment { Id = 9, ShiftId = 9,  EmployeeId = 5, ApprovedOvertime = false },
    new Assignment { Id = 10, ShiftId = 10, EmployeeId = 2, ApprovedOvertime = false },
    new Assignment { Id = 11, ShiftId = 11, EmployeeId = 3, ApprovedOvertime = false },
    new Assignment { Id = 12, ShiftId = 12, EmployeeId = 4, ApprovedOvertime = false },
    new Assignment { Id = 13, ShiftId = 13, EmployeeId = 3, ApprovedOvertime = false },
    new Assignment { Id = 14, ShiftId = 14, EmployeeId = 4, ApprovedOvertime = false }
};

    public bool CopyLastPublishedWeek(DateTime targetWeekStart)
    {
        targetWeekStart = StartOfWeek(targetWeekStart);

        var sourceWeekStart = GetPublishedWeeks()
            .Where(d => d < targetWeekStart)
            .OrderByDescending(d => d)
            .FirstOrDefault();

        if (sourceWeekStart == default)
            return false;

        var sourceWeekEnd = sourceWeekStart.AddDays(7);
        var targetWeekEnd = targetWeekStart.AddDays(7);

        var sourceShifts = Shifts
            .Where(s => s.Date >= sourceWeekStart && s.Date < sourceWeekEnd)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToList();

        if (!sourceShifts.Any())
            return false;

        var targetShiftIds = Shifts
            .Where(s => s.Date >= targetWeekStart && s.Date < targetWeekEnd)
            .Select(s => s.Id)
            .ToHashSet();

        Assignments.RemoveAll(a => targetShiftIds.Contains(a.ShiftId));
        Shifts.RemoveAll(s => s.Date >= targetWeekStart && s.Date < targetWeekEnd);

        int nextShiftId = Shifts.Any() ? Shifts.Max(s => s.Id) + 1 : 1;
        int nextAssignmentId = Assignments.Any() ? Assignments.Max(a => a.Id) + 1 : 1;

        var shiftIdMap = new Dictionary<int, int>();

        foreach (var sourceShift in sourceShifts)
        {
            var dayOffset = (sourceShift.Date.Date - sourceWeekStart.Date).Days;

            var newShift = new Shift
            {
                Id = nextShiftId++,
                CompanyId = sourceShift.CompanyId,
                Date = targetWeekStart.AddDays(dayOffset),
                Position = sourceShift.Position,
                StartTime = sourceShift.StartTime,
                EndTime = sourceShift.EndTime
            };

            Shifts.Add(newShift);
            shiftIdMap[sourceShift.Id] = newShift.Id;
        }

        var sourceShiftIds = sourceShifts.Select(s => s.Id).ToHashSet();

        var sourceAssignments = Assignments
            .Where(a => sourceShiftIds.Contains(a.ShiftId))
            .ToList();

        foreach (var sourceAssignment in sourceAssignments)
        {
            if (!shiftIdMap.TryGetValue(sourceAssignment.ShiftId, out var newShiftId))
                continue;

            Assignments.Add(new Assignment
            {
                Id = nextAssignmentId++,
                ShiftId = newShiftId,
                EmployeeId = sourceAssignment.EmployeeId,
                ApprovedOvertime = sourceAssignment.ApprovedOvertime
            });
        }

        SetWeekDraft(targetWeekStart);
        return true;
    }
}