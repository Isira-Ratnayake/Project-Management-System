using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.DataTransferObjects;
using ProjectManagementSystem.Services;

namespace ProjectManagementSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectsService _projectsService;
        public ProjectsController(ProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult ListProjects() {
            try
            {
                ListProjectsDto listProjectsDto = _projectsService.ListProjects();
                return Ok(listProjectsDto);
            }
            catch (Exception ex) { 
                string message = ex.Message;
                return Unauthorized(new { message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "CCL-1")]
        public IActionResult CreateProject([FromBody] ProjectDto projectDto)
        {
            try
            {
                string message = _projectsService.CreateProject(projectDto);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Unauthorized(new { message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "CCL-1")]
        public IActionResult UpdateProject([FromBody] ProjectDto projectDto)
        {
            try
            {
                string message = _projectsService.UpdateProject(projectDto);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Unauthorized(new { message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "CCL-1")]
        public IActionResult DeleteProject([FromBody] ProjectDto projectDto)
        {
            try
            {
                string message = _projectsService.DeleteProject(projectDto);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Unauthorized(new { message });
            }
        }
    }
}
