using ProjectManagementSystem.Repositories;

namespace ProjectManagementSystem.Services
{
    public class TasksService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TaskRepository taskRepository;

        public TasksService(IHttpContextAccessor httpContextAccessor, TaskRepository taskRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            this.taskRepository = taskRepository;
        }
    }
}
