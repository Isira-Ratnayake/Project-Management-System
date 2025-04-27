namespace ProjectManagementSystem.DataTransferObjects
{
    public class UserDto
    {
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? Token { get; set; }
        public string? UserFullname { get; set; }
        public string? UserGroupId { get; set; }
        public string? UserGroupName { get; set; }
        public string? BatchNo { get; set; }

        public UserDto(string? userId, string? userEmail, string? token, string? userFullname, string? userGroupId, string? userGroupName, string? batchNo)
        {
            UserId = userId;
            UserEmail = userEmail;
            Token = token;
            UserFullname = userFullname;
            UserGroupId = userGroupId;
            UserGroupName = userGroupName;
            BatchNo = batchNo;
        }

        public UserDto() { }
    }
}
