namespace ProjectManagementSystem.DataTransferObjects
{
    public class ProjectDto : VersionedDto
    {
        public string? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int TotalTasks { get; set; }
        public int TotalToDoTasks { get; set; }
        public int TotalInProgressTasks { get; set; }
        public int TotalInReviewTasks { get; set; }
        public int TotalDoneTasks { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

        public ProjectDto(string? batchNo, string? created, string? createdBy, string? lastModified, string? lastModifiedBy, AuthoritiesDto? authorities, string? projectId, string? projectName, int totalTasks, int totalToDoTasks, int totalInProgressTasks, int totalInReviewTasks, int totalDoneTasks, string? startDate, string? endDate) : base(batchNo, created, createdBy, lastModified, lastModifiedBy, authorities)
        {
            ProjectId = projectId;
            ProjectName = projectName;
            TotalTasks = totalTasks;
            TotalToDoTasks = totalToDoTasks;
            TotalInProgressTasks = totalInProgressTasks;
            TotalInReviewTasks = totalInReviewTasks;
            TotalDoneTasks = totalDoneTasks;
            StartDate = startDate;
            EndDate = endDate;
        }

        public ProjectDto() : base() { }
    }
}
