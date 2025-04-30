using ProjectManagementSystem.DataTransferObjects;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories;

namespace ProjectManagementSystem.Services
{
    public class UsersService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserRepository _userRepository;
        private readonly UserGroupRepository _userGroupRepository;

        public UsersService(UserRepository userRepository, UserGroupRepository userGroupRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public ListUsersDto ListUsers() {
            try {
                List<User> users = _userRepository.ReadAll();
                IEnumerable<User> userQuery = from user in users orderby user.UserId descending select user;
                List<UserDto> usersDtos = new List<UserDto>();
                foreach (User user in userQuery) {
                    AuthoritiesDto userAuthoritiesDto = new AuthoritiesDto() { 
                        IsDelete = true,
                        IsUpdate = true,
                    };
                    Audit<User> currentAudit = _userRepository.ReadAuditByCurrentBatchNo(user.CurrentBatchNo);
                    Audit<User> originalAudit = _userRepository.ReadAuditByCurrentBatchNo(user.OriginalBatchNo);
                    UserDto userDto = new UserDto(user.CurrentBatchNo,  originalAudit.ActionDateTime.ToString("yyyy-MM-dd HH:mm:ss"), originalAudit.ActionUserId, currentAudit.ActionDateTime.ToString("yyyy-MM-dd HH:mm:ss"), currentAudit.ActionUserId, userAuthoritiesDto, user.UserId, user.UserEmail, string.Empty, user.UserFullname, user.UserGroup.UserGroupId, user.UserGroup.UserGroupName);
                    usersDtos.Add(userDto);
                }
                AuthoritiesDto authoritiesDto = new AuthoritiesDto() { 
                    IsListAll = true,
                    IsCreate = true,
                };
                List<SelectionReferenceDto> userGroups = new List<SelectionReferenceDto>();
                foreach (UserGroup userGroup in _userGroupRepository.ReadAll()) { 
                    userGroups.Add(new SelectionReferenceDto(userGroup.UserGroupId, userGroup.UserGroupName));
                }
                return new ListUsersDto(authoritiesDto, usersDtos, userGroups);
            }
            catch (Exception) {
                throw new Exception("Failed to fetch data.");
            }  
        }

        public string CreateUser(UserDto userDto) {
            try {
                User user = new User(string.Empty, userDto.UserEmail ?? string.Empty, BCrypt.Net.BCrypt.EnhancedHashPassword(userDto.Token) ?? string.Empty, userDto.UserFullname ?? string.Empty, new UserGroup(userDto.UserGroupId ?? string.Empty), string.Empty, string.Empty);
                _userRepository.Create(user, GetCurrentUser().UserId);
                return "User successfully created.";
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public string UpdateUser(UserDto userDto)
        {
            try
            {
                User user = new User(userDto.UserId ?? string.Empty, userDto.UserEmail ?? string.Empty, string.Empty, userDto.UserFullname ?? string.Empty, new UserGroup(userDto.UserGroupId ?? string.Empty), string.Empty, userDto.BatchNo ?? string.Empty);
                _userRepository.Update(user, GetCurrentUser().UserId);
                return "User successfully updated.";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string DeleteUser(UserDto userDto)
        {
            try
            {
                User user = new User(userDto.UserId ?? string.Empty, string.Empty, string.Empty, string.Empty, new UserGroup(string.Empty), string.Empty, userDto.BatchNo ?? string.Empty);
                _userRepository.Delete(user, GetCurrentUser().UserId);
                return "User successfully deleted.";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private User GetCurrentUser() {
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
