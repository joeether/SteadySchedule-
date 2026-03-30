namespace SteadySchedule.Domain
{
    public class WeekTemplateShift
    {
        public int Id { get; set; }
        public int WeekTemplateId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string Position { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int Count { get; set; } = 1;
    }
}
