using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.DataTransferObjects;
using ProjectManagementSystem.Services;

namespace ProjectManagementSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService) {
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize(Roles = "CCL-1")]
        public IActionResult ListUsers() {
            try
            {
                ListUsersDto listUsersDto = _usersService.ListUsers();
                return Ok(listUsersDto);
            }
            catch (Exception ex) {
                string message = ex.Message;
                return Unauthorized(new { message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "CCL-1")]
        public IActionResult CreateUser([FromBody] UserDto userDto)
        {
            try
            {
                string message = _usersService.CreateUser(userDto);
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
        public IActionResult UpdateUser([FromBody] UserDto userDto)
        {
            try
            {
                string message = _usersService.UpdateUser(userDto);
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
        public IActionResult DeleteUser([FromBody] UserDto userDto)
        {
            try
            {
                string message = _usersService.DeleteUser(userDto);
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
