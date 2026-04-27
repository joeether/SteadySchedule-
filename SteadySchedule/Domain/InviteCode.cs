public class InviteCode
{
    public int Id { get; set; }

    public string Code { get; set; } = "";

    public DateTime ExpirationDate { get; set; }

    public bool IsUsed { get; set; }

    public int CompanyId { get; set; }

    public int? EmployeeId { get; set; } // optional but powerful
}