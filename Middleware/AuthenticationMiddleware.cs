using Microsoft.IdentityModel.JsonWebTokens;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories;
using System.Security.Claims;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserRepository userRepository) {
            if (context.User.Identity?.IsAuthenticated == true) {
                string? userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId)) {
                    try
                    {
                        User user = userRepository.ReadById(userId);
                        context.Items["User"] = user;
                    }
                    catch(Exception) { }
                }
            }
           await _next(context);
        }
    }
}
