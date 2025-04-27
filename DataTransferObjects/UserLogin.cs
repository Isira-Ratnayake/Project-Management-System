namespace ProjectManagementSystem.DataTransferObjects
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserLogin() {
            Email = string.Empty;
            Password = string.Empty;
        }
        public UserLogin(string email, string password) { 
            Email = email;
            Password = password;
        }
    }
}
