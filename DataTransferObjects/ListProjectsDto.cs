
namespace ProjectManagementSystem.DataTransferObjects
{
    public class ListProjectsDto : ListDto<ProjectDto>
    {
        public ListProjectsDto(AuthoritiesDto authorities, List<ProjectDto> objects) : base(authorities, objects) { }
    }
}
