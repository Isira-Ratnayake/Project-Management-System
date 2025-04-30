namespace ProjectManagementSystem.DataTransferObjects
{
    public class UserDto : VersionedDto
    {
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? Token { get; set; }
        public string? UserFullname { get; set; }
        public string? UserGroupId { get; set; }
        public string? UserGroupName { get; set; }

        public UserDto(string? batchNo, string? created, string? createdBy, string? lastModified, string? lastModifiedBy, AuthoritiesDto? authorities, string? userId, string? userEmail, string? token, string? userFullname, string? userGroupId, string? userGroupName) : base(batchNo, created, createdBy, lastModified, lastModifiedBy, authorities)
        {
            UserId = userId;
            UserEmail = userEmail;
            Token = token;
            UserFullname = userFullname;
            UserGroupId = userGroupId;
            UserGroupName = userGroupName;
        }

        public UserDto() : base() { }
    }
}
