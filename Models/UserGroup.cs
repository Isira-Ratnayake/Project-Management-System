namespace ProjectManagementSystem.Models
{
    public class UserGroup
    {
        public required string UserGroupId { get; set; }
        public string? UserGroupName { get; set; }
        public List<User>? Users { get; set; }
        public UserGroup(string userGroupId) {
            UserGroupId = userGroupId;
        }
        public UserGroup(string userGroupId, string userGroupName)
        {
            UserGroupId = userGroupId;
            UserGroupName = userGroupName;
        }
        public UserGroup(string userGroupId, string userGroupName, List<User> users)
        {
            UserGroupId = userGroupId;
            UserGroupName = userGroupName;
            Users = users;
        }
    }
}
