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
}
