using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystem.DataTransferObjects;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories;
using ProjectManagementSystem.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementSystem.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;

        public AuthenticationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Authentiate(UserLoginDto userLoginDto)
        {
            try
            {
                User user = _userRepository.ReadByEmail(userLoginDto.Email ?? string.Empty);
                if (BCrypt.Net.BCrypt.EnhancedVerify(userLoginDto.Password, user.UserPassword))
                {
                    return user;
                }
                throw new UnauthorizedAccessException();
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Password incorrect.");
            }
            catch (Exception)
            {
                throw new Exception("User does not exist.");
            }
        }

        public string GenerateJwtToken(User user)
        {
            Claim[] claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.UserGroup.UserGroupId)
            };
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Convert.FromHexString(SecurityConstants.PrivateKey));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "ccl.lk",
                audience: "ccl.lk",
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
