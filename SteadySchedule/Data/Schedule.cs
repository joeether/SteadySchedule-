namespace SteadySchedule.Data
{
    public class Schedule
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public DateTime WeekStart { get; set; }

        public bool IsPublished { get; set; }
    }
}
