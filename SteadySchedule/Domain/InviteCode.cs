public class InviteCode
{
    public int Id { get; set; }

    public string Code { get; set; } = "";

    public int CompanyId { get; set; }

    public int EmployeeId { get; set; }   //THIS is the key

    public DateTime ExpirationDate { get; set; }

    public bool IsUsed { get; set; }
}