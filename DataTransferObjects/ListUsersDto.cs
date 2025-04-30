namespace ProjectManagementSystem.DataTransferObjects
{
    public class ListUsersDto : ListDto<UserDto>
    {
        public List<SelectionReferenceDto>? UserGroups { get; set; }

        public ListUsersDto(AuthoritiesDto authorities, List<UserDto> objects, List<SelectionReferenceDto> userGroups) : base(authorities, objects)
        {
            UserGroups = userGroups;
        }

        public ListUsersDto() : base() { }
    }
}
