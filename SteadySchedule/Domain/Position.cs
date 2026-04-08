namespace SteadySchedule.Domain
{
    public class Position
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
