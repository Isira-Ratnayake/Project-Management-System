using System.Diagnostics.CodeAnalysis;

namespace ProjectManagementSystem.Models
{
    public class Task : Versioned
    {
        public required string TaskId { get; set; }
        public required string TaskTitle { get; set; }
        public required string TaskDescription { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required TaskStatus TaskStatus { get; set; }
        public required Project Project { get; set; }
        public List<User>? Users { get; set; }

        [SetsRequiredMembers]
        public Task(string taskId, string taskTite, string taskDescription, DateTime startDate, DateTime endDate, TaskStatus taskStatus, Project project, string originalBatchNo, string currentBatchNo) : base(originalBatchNo, currentBatchNo) { 
            TaskId = taskId;
            TaskTitle = taskTite;
            TaskDescription = taskDescription;
            StartDate = startDate;
            EndDate = endDate;
            TaskStatus = taskStatus;
            Project = project;
        }
        [SetsRequiredMembers]
        public Task(string taskId, string taskTite, string taskDescription, DateTime startDate, DateTime endDate, TaskStatus taskStatus, Project project, string originalBatchNo, string currentBatchNo, List<User> users) : base(originalBatchNo, currentBatchNo)
        {
            TaskId = taskId;
            TaskTitle = taskTite;
            TaskDescription = taskDescription;
            StartDate = startDate;
            EndDate = endDate;
            TaskStatus = taskStatus;
            Project = project;
            Users = users;
        }
    }
}
