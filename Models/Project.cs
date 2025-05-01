using System.Diagnostics.CodeAnalysis;

namespace ProjectManagementSystem.Models
{
    public class Project : Versioned
    {
        public required string ProjectId { get; set; }
        public required string ProjectName { get; set; }
        public List<Task>? Tasks { get; set; }

        [SetsRequiredMembers]
        public Project(string projectId, string projectName, string originalBatchNo, string currentBatchNo) : base(originalBatchNo, currentBatchNo) { 
            ProjectId = projectId;
            ProjectName = projectName;
        }

        [SetsRequiredMembers]
        public Project(string projectId, string projectName, List<Task> tasks, string originalBatchNo, string currentBatchNo) : base(originalBatchNo, currentBatchNo) { 
            ProjectId = projectId;
            ProjectName = projectName;
            Tasks = tasks;
        }
    }
}
