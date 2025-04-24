namespace ProjectManagementSystem.Models
{
    public class TaskStatus
    {
        public required string TaskStatusId { get; set; }
        public string? TaskStatusDescription { get; set; }

        public TaskStatus(string taskStatusId)
        {
            TaskStatusId = taskStatusId;
        }

        public TaskStatus(string taskStatusId, string taskStatusDescription) { 
            TaskStatusId = taskStatusId;
            TaskStatusDescription = taskStatusDescription;
        }
    }
}
