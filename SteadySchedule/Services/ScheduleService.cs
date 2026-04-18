using SteadySchedule.Domain;
using Microsoft.EntityFrameworkCore;
using SteadySchedule.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace SteadySchedule.Services;

public class ScheduleService
{
    private readonly AppDbContext _db;
    public ScheduleService(AppDbContext db)
    {
        _db = db;
    }


    public async Task PublishWeekAsync(DateTime weekStart)
    {
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
        weekStart = StartOfWeek(weekStart);

        var existing = await _db.Schedules
            .FirstOrDefaultAsync(s =>
                s.CompanyId == Company.Id &&
                s.WeekStart == weekStart);

        if (existing is not null)
        {
            return existing.IsPublished
                ? WeekStatus.Published
                : WeekStatus.InReview;
        }

        return WeekStatus.Draft;
    }

    public async Task<List<SteadySchedule.Data.Schedule>> GetPublishedSchedulesAsync()
    {
        return await _db.Schedules
            .Where(s => s.CompanyId == Company.Id && s.IsPublished)
            .OrderByDescending(s => s.WeekStart)
            .ToListAsync();
    }

    public List<string> Positions { get; } = new()
    {
        "Cashier",
        "Cook"
    };

    public Company Company { get; } = new()
    {
        Id = 1,
        Name = "Mama's Burger Joint",
        AdminEmail = "admin@mamasburgers.com"
    };

    public async Task<List<Position>> GetPositionsAsync()
    {

return await _db.Positions
    .Where(p => p.CompanyId == companyId)
    .OrderBy(p => p.Name)
    .ToListAsync();
    }

    public async Task SeedPositionsIfEmptyAsync()
    {
        if (await _db.Positions.AnyAsync(p => p.CompanyId == Company.Id))
            return;

        var positions = new List<Position>
    {
        new Position { CompanyId = Company.Id, Name = "Cashier" },
        new Position { CompanyId = Company.Id, Name = "Cook" }
    };

        _db.Positions.AddRange(positions);
        await _db.SaveChangesAsync();
    }

    public async Task AddPositionAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        var trimmed = name.Trim();

        var exists = await _db.Positions.AnyAsync(p =>
            p.CompanyId == Company.Id &&
            p.Name == trimmed);

        if (exists)
            return;

        _db.Positions.Add(new Position
        {
            CompanyId = Company.Id,
            Name = trimmed
        });

        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeletePositionAsync(string name)
    {
        var trimmed = name.Trim();

        var usedInShift = await _db.Shifts.AnyAsync(s =>
            s.CompanyId == Company.Id &&
            s.Position == trimmed);

        if (usedInShift)
            return false;

        var position = await _db.Positions.FirstOrDefaultAsync(p =>
            p.CompanyId == Company.Id &&
            p.Name == trimmed);

        if (position == null)
            return false;

        _db.Positions.Remove(position);
        await _db.SaveChangesAsync();
        return true;
    }

    public static DateTime StartOfWeek(DateTime date)
    {
        // Monday-based
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.Date.AddDays(-diff);
    }

    public async Task<bool> CopyLastPublishedWeekAsync(DateTime targetWeekStart)
    {
        targetWeekStart = StartOfWeek(targetWeekStart);

        var sourceWeekStart = await _db.Schedules
            .Where(s => s.CompanyId == Company.Id &&
                        s.IsPublished &&
                        s.WeekStart < targetWeekStart)
            .OrderByDescending(s => s.WeekStart)
            .Select(s => s.WeekStart)
            .FirstOrDefaultAsync();

        if (sourceWeekStart == default)
            return false;

        var sourceWeekEnd = sourceWeekStart.AddDays(7);
        var targetWeekEnd = targetWeekStart.AddDays(7);

        var sourceShifts = await _db.Shifts
            .Where(s => s.CompanyId == Company.Id &&
                        s.Date >= sourceWeekStart &&
                        s.Date < sourceWeekEnd)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync();

        if (!sourceShifts.Any())
            return false;

        var targetShifts = await _db.Shifts
            .Where(s => s.CompanyId == Company.Id &&
                        s.Date >= targetWeekStart &&
                        s.Date < targetWeekEnd)
            .ToListAsync();

        var targetShiftIds = targetShifts
            .Select(s => s.Id)
            .ToHashSet();

        var targetAssignments = await _db.Assignments
            .Where(a => a.CompanyId == Company.Id &&
                        targetShiftIds.Contains(a.ShiftId))
            .ToListAsync();

        if (targetAssignments.Any())
            _db.Assignments.RemoveRange(targetAssignments);

        if (targetShifts.Any())
            _db.Shifts.RemoveRange(targetShifts);

        await _db.SaveChangesAsync();

        var shiftIdMap = new Dictionary<int, int>();
        var newShifts = new List<Shift>();

        foreach (var sourceShift in sourceShifts)
        {
            var dayOffset = (sourceShift.Date.Date - sourceWeekStart.Date).Days;

            var newShift = new Shift
            {
                CompanyId = Company.Id,
                Date = targetWeekStart.AddDays(dayOffset),
                Position = sourceShift.Position,
                StartTime = sourceShift.StartTime,
                EndTime = sourceShift.EndTime,
                SlotGroupId = sourceShift.SlotGroupId
            };

            newShifts.Add(newShift);
            shiftIdMap[sourceShift.Id] = 0; // placeholder until IDs are generated
        }

        _db.Shifts.AddRange(newShifts);
        await _db.SaveChangesAsync();

        for (int i = 0; i < sourceShifts.Count; i++)
        {
            shiftIdMap[sourceShifts[i].Id] = newShifts[i].Id;
        }

        var sourceShiftIds = sourceShifts
            .Select(s => s.Id)
            .ToHashSet();

        var sourceAssignments = await _db.Assignments
            .Where(a => a.CompanyId == Company.Id &&
                        sourceShiftIds.Contains(a.ShiftId))
            .ToListAsync();

        var newAssignments = new List<Assignment>();

        foreach (var sourceAssignment in sourceAssignments)
        {
            if (!shiftIdMap.TryGetValue(sourceAssignment.ShiftId, out var newShiftId))
                continue;

            newAssignments.Add(new Assignment
            {
                CompanyId = Company.Id,
                ShiftId = newShiftId,
                EmployeeId = sourceAssignment.EmployeeId,
                ApprovedOvertime = sourceAssignment.ApprovedOvertime
            });
        }

        if (newAssignments.Any())
        {
            _db.Assignments.AddRange(newAssignments);
            await _db.SaveChangesAsync();
        }

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

    public async Task<bool> ShiftHasAssignmentsAsync(int shiftId)
    {
        return await _db.Assignments
            .AnyAsync(a => a.CompanyId == Company.Id && a.ShiftId == shiftId);
    }

    public async Task<bool> PositionIsUsedInShiftsAsync(string position)
    {
        return await _db.Shifts.AnyAsync(s =>
            s.CompanyId == Company.Id &&
            s.Position == position);
    }

    public async Task ClearWeekAsync(DateTime weekStart)
    {
        weekStart = StartOfWeek(weekStart);
        var weekEnd = weekStart.AddDays(7);

        var weekShifts = await _db.Shifts
            .Where(s => s.CompanyId == Company.Id &&
                        s.Date >= weekStart &&
                        s.Date < weekEnd)
            .ToListAsync();

        var weekShiftIds = weekShifts.Select(s => s.Id).ToHashSet();

        var weekAssignments = await _db.Assignments
            .Where(a => a.CompanyId == Company.Id &&
                        weekShiftIds.Contains(a.ShiftId))
            .ToListAsync();

        if (weekAssignments.Any())
            _db.Assignments.RemoveRange(weekAssignments);

        if (weekShifts.Any())
            _db.Shifts.RemoveRange(weekShifts);

        await _db.SaveChangesAsync();
    }

    public async Task<List<WeekTemplate>> GetWeekTemplatesAsync()
    {
        return await _db.WeekTemplates
            .Where(t => t.CompanyId == Company.Id)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<WeekTemplateShift>> GetWeekTemplateShiftsAsync()
    {
        return await _db.WeekTemplateShifts
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<WeekTemplate> AddWeekTemplateAsync(string name)
    {
        var template = new WeekTemplate
        {
            CompanyId = Company.Id,
            Name = name.Trim()
        };

        _db.WeekTemplates.Add(template);
        await _db.SaveChangesAsync();
        return template;
    }

    public async Task AddWeekTemplateShiftAsync(WeekTemplateShift shift)
    {
        _db.WeekTemplateShifts.Add(shift);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateWeekTemplateShiftAsync(WeekTemplateShift updatedShift)
    {
        var existing = await _db.WeekTemplateShifts.FirstOrDefaultAsync(s => s.Id == updatedShift.Id);
        if (existing == null) return;

        existing.DayOfWeek = updatedShift.DayOfWeek;
        existing.Position = updatedShift.Position;
        existing.StartTime = updatedShift.StartTime;
        existing.EndTime = updatedShift.EndTime;
        existing.Count = updatedShift.Count;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteWeekTemplateShiftAsync(int id)
    {
        var shift = await _db.WeekTemplateShifts.FirstOrDefaultAsync(s => s.Id == id);
        if (shift == null) return;

        _db.WeekTemplateShifts.Remove(shift);
        await _db.SaveChangesAsync();
    }

    public async Task RenameWeekTemplateAsync(int id, string newName)
    {
        var template = await _db.WeekTemplates.FirstOrDefaultAsync(t => t.Id == id && t.CompanyId == Company.Id);
        if (template == null) return;

        template.Name = newName.Trim();
        await _db.SaveChangesAsync();
    }

    public async Task DeleteWeekTemplateAsync(int id)
    {
        var template = await _db.WeekTemplates.FirstOrDefaultAsync(t => t.Id == id && t.CompanyId == Company.Id);
        if (template == null) return;

        var shifts = await _db.WeekTemplateShifts.Where(s => s.WeekTemplateId == id).ToListAsync();
        if (shifts.Any())
            _db.WeekTemplateShifts.RemoveRange(shifts);

        _db.WeekTemplates.Remove(template);
        await _db.SaveChangesAsync();
    }
}