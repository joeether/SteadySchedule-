namespace SteadySchedule.Domain;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;

    // CONTROL
    public bool IsActive { get; set; } = true;

    // TRIAL / BILLING (light version)
    public DateTime TrialStartDate { get; set; } = DateTime.UtcNow;
    public DateTime TrialEndDate { get; set; } = DateTime.UtcNow.AddDays(30);

    public string Status { get; set; } = "Trialing"; // Trialing / Active / PastDue / Inactive

    public int BillingAnchorDay { get; set; } = DateTime.UtcNow.Day;
}
