using SteadySchedule.Domain;
using Microsoft.EntityFrameworkCore;
using SteadySchedule.Data;
namespace SteadySchedule.Services;


public class ScheduleService
{
    private readonly AppDbContext _db;

    public ScheduleService(AppDbContext db)
    {
        _db = db;
        WeekStatuses[SeedWeekStart] = WeekStatus.Published;
    }

    public Dictionary<DateTime, WeekStatus> WeekStatuses { get; } = new();

    private static readonly DateTime SeedWeekStart = StartOfWeek(DateTime.Today);

    public async Task PublishWeekAsync(DateTime weekStart)
    {
        PublishWeek(weekStart);

        var existing = await _db.Schedules
            .FirstOrDefaultAsync(s =>
                s.CompanyId == Company.Id &&
                s.WeekStart == weekStart);

        if (existing == null)
        {
            _db.Schedules.Add(new SteadySchedule.Data.Schedule
            {
                CompanyId = Company.Id,
                WeekStart = weekStart,
                IsPublished = true
            });
        }
        else
        {
            existing.IsPublished = true;
        }

        await _db.SaveChangesAsync();
    }

    public async Task UnpublishWeekAsync(DateTime weekStart)
    {
        UnpublishWeek(weekStart);

        var existing = await _db.Schedules
            .FirstOrDefaultAsync(s =>
                s.CompanyId == Company.Id &&
                s.WeekStart == weekStart);

        if (existing != null)
        {
            existing.IsPublished = false;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<WeekStatus> GetWeekStatusAsync(DateTime weekStart)
    {
        var existing = await _db.Schedules
            .FirstOrDefaultAsync(s =>
                s.CompanyId == Company.Id &&
                s.WeekStart == weekStart);

        if (existing is not null)
        {
            WeekStatuses[weekStart] = existing.IsPublished
                ? WeekStatus.Published
                : WeekStatus.InReview;
        }

        return GetWeekStatus(weekStart);
    }

    public async Task LoadWeekStatusFromDbAsync(DateTime weekStart)
    {
        var existing = await _db.Schedules
            .FirstOrDefaultAsync(s =>
                s.CompanyId == Company.Id &&
                s.WeekStart == weekStart);

        if (existing is not null)
        {
            WeekStatuses[weekStart] = existing.IsPublished
                ? WeekStatus.Published
                : WeekStatus.InReview;
        }
    }

    public async Task<List<SteadySchedule.Data.Schedule>> GetPublishedSchedulesAsync()
    {
        return await _db.Schedules
            .Where(s => s.CompanyId == Company.Id && s.IsPublished)
            .OrderByDescending(s => s.WeekStart)
            .ToListAsync();
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

    //public List<WeekTemplate> WeekTemplates { get; set; } = new();
    //public List<WeekTemplateShift> WeekTemplateShifts { get; set; } = new();

    // TEMP test data for development - remove before release
    public List<WeekTemplate> WeekTemplates { get; set; } = new()
    {
        new WeekTemplate
        {
            Id = 1,
            CompanyId = 1,
            Name = "Week A"
        }
    };
    // TEMP test data for development - remove before release
    public List<WeekTemplateShift> WeekTemplateShifts { get; set; } = new()
    {
        new WeekTemplateShift
        {
            Id = 1,
            WeekTemplateId = 1,
            DayOfWeek = DayOfWeek.Monday,
            Position = "Cashier",
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            Count = 3
        },
        new WeekTemplateShift
        {
            Id = 2,
            WeekTemplateId = 1,
            DayOfWeek = DayOfWeek.Monday,
            Position = "Cook",
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            Count = 1
        },
        new WeekTemplateShift
        {
            Id = 3,
            WeekTemplateId = 1,
            DayOfWeek = DayOfWeek.Tuesday,
            Position = "Cashier",
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            Count = 2
        },
        new WeekTemplateShift
        {
            Id = 4,
            WeekTemplateId = 1,
            DayOfWeek = DayOfWeek.Wednesday,
            Position = "Cook",
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            Count = 2
        }
    };
    // TEMP test data for development - remove before release
    public List<Shift> Shifts { get; } = new()
    {
        // Monday
        new Shift { Id = 1, CompanyId = 1, Date = SeedWeekStart.AddDays(0), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 2, CompanyId = 1, Date = SeedWeekStart.AddDays(0), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 3, CompanyId = 1, Date = SeedWeekStart.AddDays(0), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

        // Tuesday
        new Shift { Id = 4, CompanyId = 1, Date = SeedWeekStart.AddDays(1), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 5, CompanyId = 1, Date = SeedWeekStart.AddDays(1), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 6, CompanyId = 1, Date = SeedWeekStart.AddDays(1), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

        // Wednesday
        new Shift { Id = 7, CompanyId = 1, Date = SeedWeekStart.AddDays(2), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 8, CompanyId = 1, Date = SeedWeekStart.AddDays(2), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 9, CompanyId = 1, Date = SeedWeekStart.AddDays(2), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

        // Thursday
        new Shift { Id = 10, CompanyId = 1, Date = SeedWeekStart.AddDays(3), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 11, CompanyId = 1, Date = SeedWeekStart.AddDays(3), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 12, CompanyId = 1, Date = SeedWeekStart.AddDays(3), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

        // Friday
        new Shift { Id = 13, CompanyId = 1, Date = SeedWeekStart.AddDays(4), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 14, CompanyId = 1, Date = SeedWeekStart.AddDays(4), Position = "Cashier", StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(13,0,0) },
        new Shift { Id = 15, CompanyId = 1, Date = SeedWeekStart.AddDays(4), Position = "Cook",    StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(18,0,0) },

        // Saturday
        new Shift { Id = 16, CompanyId = 1, Date = SeedWeekStart.AddDays(5), Position = "Cashier", StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(14,0,0) },
        new Shift { Id = 17, CompanyId = 1, Date = SeedWeekStart.AddDays(5), Position = "Cashier", StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(14,0,0) },
        new Shift { Id = 18, CompanyId = 1, Date = SeedWeekStart.AddDays(5), Position = "Cook",    StartTime = new TimeSpan(11,0,0), EndTime = new TimeSpan(19,0,0) },

        // Sunday
        new Shift { Id = 19, CompanyId = 1, Date = SeedWeekStart.AddDays(6), Position = "Cashier", StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(14,0,0) },
        new Shift { Id = 20, CompanyId = 1, Date = SeedWeekStart.AddDays(6), Position = "Cashier", StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(14,0,0) },
        new Shift { Id = 21, CompanyId = 1, Date = SeedWeekStart.AddDays(6), Position = "Cook",    StartTime = new TimeSpan(11,0,0), EndTime = new TimeSpan(19,0,0) }
    };
    // TEMP test data for development - remove before release
    public List<Assignment> Assignments { get; } = new()
    {
        new Assignment { Id = 1, ShiftId = 1,  EmployeeId = 1, ApprovedOvertime = false },
        new Assignment { Id = 2, ShiftId = 3,  EmployeeId = 2, ApprovedOvertime = false },

        new Assignment { Id = 3, ShiftId = 4,  EmployeeId = 1, ApprovedOvertime = false },
        new Assignment { Id = 4, ShiftId = 6,  EmployeeId = 2, ApprovedOvertime = false },

        new Assignment { Id = 5, ShiftId = 7,  EmployeeId = 5, ApprovedOvertime = false },
        new Assignment { Id = 6, ShiftId = 9,  EmployeeId = 4, ApprovedOvertime = false },

        new Assignment { Id = 7, ShiftId = 10, EmployeeId = 1, ApprovedOvertime = false },
        new Assignment { Id = 8, ShiftId = 12, EmployeeId = 4, ApprovedOvertime = false },

        new Assignment { Id = 9, ShiftId = 13, EmployeeId = 5, ApprovedOvertime = false },
        new Assignment { Id = 10, ShiftId = 15, EmployeeId = 2, ApprovedOvertime = false },

        new Assignment { Id = 11, ShiftId = 16, EmployeeId = 3, ApprovedOvertime = false },
        new Assignment { Id = 12, ShiftId = 18, EmployeeId = 4, ApprovedOvertime = false },

        new Assignment { Id = 13, ShiftId = 19, EmployeeId = 3, ApprovedOvertime = false },
        new Assignment { Id = 14, ShiftId = 21, EmployeeId = 4, ApprovedOvertime = false }
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

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await _db.Employees
            .Where(e => e.CompanyId == Company.Id)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task SeedEmployeesIfEmptyAsync()
    {
        if (await _db.Employees.AnyAsync(e => e.CompanyId == Company.Id))
            return;

        var employees = new List<Employee>
    {
        new Employee
        {
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
            CompanyId = 1,
            Name = "Carla",
            MaxHoursPerWeek = 24,
            PositionsQualified = "Cashier",
            SaturdayAvailable = true, SaturdayStart = new TimeSpan(10,0,0), SaturdayEnd = new TimeSpan(16,0,0),
            SundayAvailable = true, SundayStart = new TimeSpan(10,0,0), SundayEnd = new TimeSpan(16,0,0)
        },
        new Employee
        {
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
            CompanyId = 1,
            Name = "Emma",
            MaxHoursPerWeek = 20,
            PositionsQualified = "Cashier",
            MondayAvailable = true, MondayStart = new TimeSpan(9,0,0), MondayEnd = new TimeSpan(13,0,0),
            WednesdayAvailable = true, WednesdayStart = new TimeSpan(9,0,0), WednesdayEnd = new TimeSpan(13,0,0),
            FridayAvailable = true, FridayStart = new TimeSpan(9,0,0), FridayEnd = new TimeSpan(13,0,0)
        }
    };

        _db.Employees.AddRange(employees);
        await _db.SaveChangesAsync();
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        employee.CompanyId = Company.Id;
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateEmployeeAsync(Employee updatedEmployee)
    {
        var existing = await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == updatedEmployee.Id && e.CompanyId == Company.Id);

        if (existing == null)
            return;

        existing.Name = updatedEmployee.Name;
        existing.Email = updatedEmployee.Email;
        existing.MaxHoursPerWeek = updatedEmployee.MaxHoursPerWeek;
        existing.PositionsQualified = updatedEmployee.PositionsQualified;

        existing.MondayAvailable = updatedEmployee.MondayAvailable;
        existing.MondayAnyTime = updatedEmployee.MondayAnyTime;
        existing.MondayStart = updatedEmployee.MondayStart;
        existing.MondayEnd = updatedEmployee.MondayEnd;

        existing.TuesdayAvailable = updatedEmployee.TuesdayAvailable;
        existing.TuesdayAnyTime = updatedEmployee.TuesdayAnyTime;
        existing.TuesdayStart = updatedEmployee.TuesdayStart;
        existing.TuesdayEnd = updatedEmployee.TuesdayEnd;

        existing.WednesdayAvailable = updatedEmployee.WednesdayAvailable;
        existing.WednesdayAnyTime = updatedEmployee.WednesdayAnyTime;
        existing.WednesdayStart = updatedEmployee.WednesdayStart;
        existing.WednesdayEnd = updatedEmployee.WednesdayEnd;

        existing.ThursdayAvailable = updatedEmployee.ThursdayAvailable;
        existing.ThursdayAnyTime = updatedEmployee.ThursdayAnyTime;
        existing.ThursdayStart = updatedEmployee.ThursdayStart;
        existing.ThursdayEnd = updatedEmployee.ThursdayEnd;

        existing.FridayAvailable = updatedEmployee.FridayAvailable;
        existing.FridayAnyTime = updatedEmployee.FridayAnyTime;
        existing.FridayStart = updatedEmployee.FridayStart;
        existing.FridayEnd = updatedEmployee.FridayEnd;

        existing.SaturdayAvailable = updatedEmployee.SaturdayAvailable;
        existing.SaturdayAnyTime = updatedEmployee.SaturdayAnyTime;
        existing.SaturdayStart = updatedEmployee.SaturdayStart;
        existing.SaturdayEnd = updatedEmployee.SaturdayEnd;

        existing.SundayAvailable = updatedEmployee.SundayAvailable;
        existing.SundayAnyTime = updatedEmployee.SundayAnyTime;
        existing.SundayStart = updatedEmployee.SundayStart;
        existing.SundayEnd = updatedEmployee.SundayEnd;

        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        var hasAssignments = await _db.Assignments
            .AnyAsync(a => a.EmployeeId == employeeId && a.CompanyId == Company.Id);

        if (hasAssignments)
            return false;

        var employee = await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId && e.CompanyId == Company.Id);

        if (employee == null)
            return false;

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Shift>> GetShiftsAsync()
    {
        return await _db.Shifts
            .Where(s => s.CompanyId == Company.Id)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task AddShiftAsync(Shift shift)
    {
        shift.CompanyId = Company.Id;
        _db.Shifts.Add(shift);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateShiftAsync(Shift updatedShift)
    {
        var existing = await _db.Shifts
            .FirstOrDefaultAsync(s => s.Id == updatedShift.Id && s.CompanyId == Company.Id);

        if (existing == null)
            return;

        existing.Date = updatedShift.Date;
        existing.Position = updatedShift.Position;
        existing.StartTime = updatedShift.StartTime;
        existing.EndTime = updatedShift.EndTime;
        existing.SlotGroupId = updatedShift.SlotGroupId;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteShiftAsync(int shiftId)
    {
        var shift = await _db.Shifts
            .FirstOrDefaultAsync(s => s.Id == shiftId && s.CompanyId == Company.Id);

        if (shift == null)
            return;

        var assignments = await _db.Assignments
            .Where(a => a.ShiftId == shiftId && a.CompanyId == Company.Id)
            .ToListAsync();

        if (assignments.Any())
            _db.Assignments.RemoveRange(assignments);

        _db.Shifts.Remove(shift);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Assignment>> GetAssignmentsAsync()
    {
        return await _db.Assignments
            .Where(a => a.CompanyId == Company.Id)
            .Include(a => a.Employee)
            .Include(a => a.Shift)
            .ToListAsync();
    }

    public async Task AddAssignmentAsync(Assignment assignment)
    {
        assignment.CompanyId = Company.Id;
        _db.Assignments.Add(assignment);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAssignmentAsync(int assignmentId)
    {
        var assignment = await _db.Assignments
            .FirstOrDefaultAsync(a => a.Id == assignmentId && a.CompanyId == Company.Id);

        if (assignment == null)
            return;

        _db.Assignments.Remove(assignment);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Shift>> GetWeekShiftsAsync(int companyId, DateTime weekStart)
    {
        weekStart = StartOfWeek(weekStart);
        var weekEnd = weekStart.AddDays(7);

        return await _db.Shifts
            .Where(s => s.CompanyId == companyId &&
                        s.Date >= weekStart &&
                        s.Date < weekEnd)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Assignment>> GetWeekAssignmentsAsync(int companyId, DateTime weekStart)
    {
        weekStart = StartOfWeek(weekStart);
        var weekEnd = weekStart.AddDays(7);

        return await _db.Assignments
            .Include(a => a.Employee)
            .Include(a => a.Shift)
            .Where(a => a.CompanyId == companyId &&
                        a.Shift != null &&
                        a.Shift.Date >= weekStart &&
                        a.Shift.Date < weekEnd)
            .OrderBy(a => a.Shift!.Date)
            .ThenBy(a => a.Shift!.StartTime)
            .ToListAsync();
    }
}