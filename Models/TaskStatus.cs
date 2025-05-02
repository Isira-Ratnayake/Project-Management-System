using System.Diagnostics.CodeAnalysis;

namespace ProjectManagementSystem.Models
{
    public class TaskStatus
    {
        public required string TaskStatusId { get; set; }
        public string? TaskStatusDescription { get; set; }

        [SetsRequiredMembers]
        public TaskStatus(string taskStatusId)
        {
            TaskStatusId = taskStatusId;
        }

        [SetsRequiredMembers]
        public TaskStatus(string taskStatusId, string taskStatusDescription) { 
            TaskStatusId = taskStatusId;
            TaskStatusDescription = taskStatusDescription;
        }
    }
}
