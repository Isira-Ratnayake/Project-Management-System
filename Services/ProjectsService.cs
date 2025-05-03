using ProjectManagementSystem.DataTransferObjects;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories;

namespace ProjectManagementSystem.Services
{
    public class ProjectsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProjectRepository _projectRepository;

        public ProjectsService(IHttpContextAccessor httpContextAccessor, ProjectRepository projectRepository) { 
            _httpContextAccessor = httpContextAccessor;
            _projectRepository = projectRepository;
        }

        public ListProjectsDto ListProjects() {
            try {
                User currentUser = GetCurrentUser();
                List<Project> projects = _projectRepository.ReadAll();
                IEnumerable<Project> projectQuery = from project in projects orderby project.ProjectId descending select project;
                List<ProjectDto> projectDtos = new List<ProjectDto>();
                foreach (Project project in projectQuery) {
                    AuthoritiesDto projectAuthoritiesDto = new AuthoritiesDto()
                    {
                        IsDelete = HasAuthority("DELETE", currentUser),
                        IsUpdate = HasAuthority("UPDATE", currentUser),
                    };
                    Audit<Project> currentAudit = _projectRepository.ReadAuditByCurrentBatchNo(project.CurrentBatchNo);
                    Audit<Project> originalAudit = _projectRepository.ReadAuditByCurrentBatchNo(project.OriginalBatchNo);
                    int totalTasks = 0;
                    int totalToDoTasks = 0;
                    int totalInProgressTasks = 0;
                    int totalInReviewTasks = 0;
                    int totalDoneTasks = 0;
                    if (project.Tasks == null) { 
                        project.Tasks = new List<Models.Task>();
                    }

                    DateTime startDate =DateTime.MaxValue;
                    DateTime endDate = DateTime.MinValue;
                    foreach (Models.Task task in project.Tasks) {
                        totalTasks++;
                        switch (task.TaskStatus.TaskStatusId) {
                            case "CCL-1": totalToDoTasks++; break;
                            case "CCL-2": totalInProgressTasks++; break;
                            case "CCL-3": totalInReviewTasks++; break;
                            case "CCL-4": totalDoneTasks++; break;
                        }
                        if (task.StartDate.CompareTo(startDate) < 0) { 
                            startDate = task.StartDate;
                        }
                        if (task.EndDate.CompareTo(endDate) > 0) { 
                            endDate = task.EndDate;
                        }
                    }
                    ProjectDto projectDto = new ProjectDto(project.CurrentBatchNo, originalAudit.ActionDateTime.ToString("yyyy-MM-dd HH:mm:ss"), originalAudit.ActionUserId, currentAudit.ActionDateTime.ToString("yyyy-MM-dd HH:mm:ss"), currentAudit.ActionUserId, projectAuthoritiesDto, project.ProjectId, project.ProjectName, totalTasks, totalToDoTasks, totalInProgressTasks, totalInReviewTasks, totalDoneTasks, totalTasks != 0 ? startDate.ToString("yyyy-MM-dd") : string.Empty, totalTasks != 0 ? endDate.ToString("yyyy-MM-dd") : string.Empty);
                    projectDtos.Add(projectDto);
                }
                AuthoritiesDto authoritiesDto = new AuthoritiesDto()
                {
                    IsListAll = HasAuthority("LIST", currentUser),
                    IsCreate = HasAuthority("CREATE", currentUser)
                };
                return new ListProjectsDto(authoritiesDto, projectDtos);
            }
            catch (Exception) {
                throw new Exception("Failed to fetch data.");
            }
        }

        public string CreateProject(ProjectDto projectDto)
        {
            try
            {
                Project project = new Project(string.Empty, projectDto.ProjectName ?? string.Empty, string.Empty, string.Empty);
                _projectRepository.Create(project, GetCurrentUser().UserId);
                return "Project successfully created.";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string UpdateProject(ProjectDto projectDto)
        {
            try
            {
                Project project = new Project(projectDto.ProjectId ?? string.Empty, projectDto.ProjectName ?? string.Empty, string.Empty, projectDto.BatchNo ?? string.Empty);
                _projectRepository.Update(project, GetCurrentUser().UserId);
                return "Project successfully updated.";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string DeleteProject(ProjectDto projectDto)
        {
            try
            {
                Project project = new Project(projectDto.ProjectId ?? string.Empty, projectDto.ProjectName ?? string.Empty, string.Empty, projectDto.BatchNo ?? string.Empty);
                _projectRepository.Delete(project, GetCurrentUser().UserId);
                return "Project successfully deleted.";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool HasAuthority(string privilegedAction, User user)
        {
            if (user.UserGroup.UserGroupId.Equals("CCL-1"))
            {
                return true;
            }
            else
            {
                if (privilegedAction.Equals("LIST")) {
                    return true;
                }
                return false;
            }
        }

        private User GetCurrentUser()
        {
            Object? obj = _httpContextAccessor.HttpContext?.Items["User"];
            if (obj != null)
            {
                User user = (User)obj;
                return user;
            }
            throw new Exception("You are not authorized for this action.");
        }
    }
}
