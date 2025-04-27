using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.DataTransferObjects;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Services;

namespace ProjectManagementSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(AuthenticationService authenticationService) { 
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin) {
            try
            {
                User user = _authenticationService.Authentiate(userLogin);
                string jwtToken = _authenticationService.GenerateJwtToken(user);
                UserDto userDto = new UserDto()
                {
                    UserId = user.UserId,
                    UserEmail = user.UserEmail,
                    Token = jwtToken,
                    UserFullname = user.UserFullname,
                    UserGroupId = user.UserGroup.UserGroupId,
                    UserGroupName = user.UserGroup.UserGroupName,
                };
                return Ok(new { userDto });
            }
            catch (Exception ex) {
                string message = ex.Message;
                return Unauthorized(new {message});
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult RequireAuth() {
            try
            {
                string message = "Session active.";
                return Ok(new { message });
            }
            catch (Exception ex) {
                string message = ex.Message;
                return Unauthorized(new { message });
            }
        }
    }
}
