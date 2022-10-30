namespace TodoApi.Models 
{
    public class Workers
    {
        public int WorkerId { get; set; }
        public string? WorkerFullname { get; set; }
        public int JobId { get; set; }
        public bool IsClassroomTeacher { get; set; }
    }
}