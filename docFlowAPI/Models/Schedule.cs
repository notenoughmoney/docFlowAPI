namespace docFlowAPI.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public int DocTypeId { get; set; }
        public int JobId { get; set; }
        public int PeriodId { get; set; }
    }
}
