namespace SteadySchedule.ViewModels
{
    public class CompanyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AdminEmail { get; set; }
        public DateTime TrialEndDate { get; set; }
public int BillingAnchorDay { get; set; }
public DateTime NextBillingDate { get; set; }
        public bool IsActive { get; set; }
        public int EmployeeCount { get; set; }
    }
}