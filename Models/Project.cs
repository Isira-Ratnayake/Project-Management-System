namespace ProjectManagementSystem.Models
{
    public class Project
    {
        public required string ProjectId { get; set; }
        public required string ProjectName { get; set; }
        public List<Task>? Tasks { get; set; }

        public Project(string projectId, string projectName) { 
            ProjectId = projectId;
            ProjectName = projectName;
        }

        public Project(string projectId, string projectName, List<Task> tasks)
        {
            ProjectId = projectId;
            ProjectName = projectName;
            Tasks = tasks;
        }
    }
}
