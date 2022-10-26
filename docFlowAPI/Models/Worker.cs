namespace TodoApi.Models 
{
    public class workers
    {
        public int WorkerId { get; set; }
        public string WorkerFullname { get; set; }
        public int JobId { get; set; }
        public string Job { get; set; }
        public bool IsClassroomTeacher { get; set; }

    }
}