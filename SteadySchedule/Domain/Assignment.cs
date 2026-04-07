namespace SteadySchedule.Domain;

public class Assignment
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int ShiftId { get; set; }
    public int EmployeeId { get; set; }
    public bool ApprovedOvertime { get; set; }

    public Shift? Shift { get; set; }
    public Employee? Employee { get; set; }
}