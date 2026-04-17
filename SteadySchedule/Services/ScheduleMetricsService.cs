using SteadySchedule.Domain;

namespace SteadySchedule.Services;

public class ScheduleMetricsService
{
    public static double GetShiftHours(Shift shift)
    {
        var start = shift.StartTime;
        var end = shift.EndTime;

        if (end < start)
            return (TimeSpan.FromHours(24) - start + end).TotalHours;

        if (end == start)
            return 24;

        return (end - start).TotalHours;
    }

    public static int GetWeeklyHours(
        Employee employee,
        List<Assignment> assignments,
        List<Shift> shifts,
        DateTime weekStart)
    {
        var weekEnd = weekStart.AddDays(7);

        var employeeAssignments = assignments
            .Where(a => a.EmployeeId == employee.Id)
            .Join(
                shifts,
                a => a.ShiftId,
                s => s.Id,
                (a, s) => s)
            .Where(s => s.Date >= weekStart && s.Date < weekEnd);

        return (int)employeeAssignments.Sum(s => GetShiftHours(s));
    }

    public static bool WouldCauseOvertimeIfAssigned(
        Employee employee,
        Shift shift,
        List<Assignment> assignments,
        List<Shift> shifts,
        DateTime weekStart)
    {
        var currentHours = GetWeeklyHours(employee, assignments, shifts, weekStart);
        var shiftHours = GetShiftHours(shift);
        return currentHours + shiftHours > employee.MaxHoursPerWeek;
    }

    private Dictionary<DateTime, Dictionary<string, List<string>>> GetWeeklyWarnings()
	{
		var result = new Dictionary<DateTime, Dictionary<string, List<string>>>();

		var weekShifts = shifts
			.Where(s => s.Date >= weekStart && s.Date < WeekEnd);

		foreach (var shift in weekShifts)
		{
			foreach (var a in assignments.Where(a => a.ShiftId == shift.Id))
			{
				var emp = employees.First(x => x.Id == a.EmployeeId);
				var warnings = GetWarnings(emp, shift, isPreview: false);

				if (!warnings.Any()) continue;

				if (!result.ContainsKey(shift.Date))
					result[shift.Date] = new Dictionary<string, List<string>>();

				if (!result[shift.Date].ContainsKey(emp.Name))
					result[shift.Date][emp.Name] = new List<string>();

				result[shift.Date][emp.Name].AddRange(warnings);
			}
		}

		return result;
	}

private static bool IsAvailableForShift(Employee e, Shift shift)
	{
		bool dayAvailable;
		bool anyTime;
		TimeSpan? start;
		TimeSpan? end;

		switch (shift.Date.DayOfWeek)
		{
			case DayOfWeek.Monday:
				dayAvailable = e.MondayAvailable;
				anyTime = e.MondayAnyTime;
				start = e.MondayStart;
				end = e.MondayEnd;
				break;
			case DayOfWeek.Tuesday:
				dayAvailable = e.TuesdayAvailable;
				anyTime = e.TuesdayAnyTime;
				start = e.TuesdayStart;
				end = e.TuesdayEnd;
				break;
			case DayOfWeek.Wednesday:
				dayAvailable = e.WednesdayAvailable;
				anyTime = e.WednesdayAnyTime;
				start = e.WednesdayStart;
				end = e.WednesdayEnd;
				break;
			case DayOfWeek.Thursday:
				dayAvailable = e.ThursdayAvailable;
				anyTime = e.ThursdayAnyTime;
				start = e.ThursdayStart;
				end = e.ThursdayEnd;
				break;
			case DayOfWeek.Friday:
				dayAvailable = e.FridayAvailable;
				anyTime = e.FridayAnyTime;
				start = e.FridayStart;
				end = e.FridayEnd;
				break;
			case DayOfWeek.Saturday:
				dayAvailable = e.SaturdayAvailable;
				anyTime = e.SaturdayAnyTime;
				start = e.SaturdayStart;
				end = e.SaturdayEnd;
				break;
			case DayOfWeek.Sunday:
				dayAvailable = e.SundayAvailable;
				anyTime = e.SundayAnyTime;
				start = e.SundayStart;
				end = e.SundayEnd;
				break;
			default:
				return true;
		}

		if (!dayAvailable)
			return false;

		if (anyTime)
			return true;

		if (!start.HasValue && !end.HasValue)
			return true;

		if (!start.HasValue || !end.HasValue)
			return false;

		return shift.StartTime >= start.Value && shift.EndTime <= end.Value;
}
}
