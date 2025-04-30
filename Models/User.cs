using System.Diagnostics.CodeAnalysis;

namespace ProjectManagementSystem.Models
{
    public class User : Versioned
    {
        public required string UserId { get; set; }
        public required string UserEmail { get; set; }
        public required string UserPassword { get; set; }
        public required string UserFullname { get; set; }
        public UserGroup UserGroup { get; set; }
        public List<Task>? Tasks { get; set; }

        [SetsRequiredMembers]
        public User(string userId, string userEmail, string userPassword, string userFullname, UserGroup userGroup, string originalBatchNo, string currentBatchNo) : base(originalBatchNo, currentBatchNo) { 
            UserId = userId;
            UserEmail = userEmail;
            UserPassword = userPassword;
            UserFullname = userFullname;
            UserGroup = userGroup;
        }

        [SetsRequiredMembers]
        public User(string userId, string userEmail, string userPassword, string userFullname, UserGroup userGroup, string originalBatchNo, string currentBatchNo, List<Task> tasks) : base(originalBatchNo, currentBatchNo)
        {
            UserId = userId;
            UserEmail = userEmail;
            UserPassword = userPassword;
            UserFullname = userFullname;
            UserGroup = userGroup;
            Tasks = tasks;
        }
    }
}
