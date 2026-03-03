using SteadySchedule.Domain;

namespace SteadySchedule.Services;

public class ScheduleService
{
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

    public List<Shift> Shifts { get; } = new()
    {
        new Shift { Id = 1, CompanyId = 1, Date = DateTime.Today, Position="Cashier", StartTime=new TimeSpan(9,0,0), EndTime=new TimeSpan(13,0,0) },
        new Shift { Id = 2, CompanyId = 1, Date = DateTime.Today, Position="Cook", StartTime=new TimeSpan(10,0,0), EndTime=new TimeSpan(18,0,0) },
        new Shift { Id = 3, CompanyId = 1, Date = DateTime.Today, Position="Cashier", StartTime=new TimeSpan(12,0,0), EndTime=new TimeSpan(20,0,0) },
        new Shift { Id = 4, CompanyId = 1, Date = DateTime.Today.AddDays(1), Position="Cashier", StartTime=new TimeSpan(12,0,0), EndTime=new TimeSpan(20,0,0) },
    };

    public List<Assignment> Assignments { get; } = new();
}