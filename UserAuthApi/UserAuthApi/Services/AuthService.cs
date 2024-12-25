using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserAuthApi.Repositories;
using UserAuthApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace UserAuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return null;

            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verificationResult != PasswordVerificationResult.Success)
                return null;

            // Ensure user.Role is properly assigned and debug it
            Console.WriteLine($"User Role: {user.Role}");

            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.Name, user.UserName),
                  new Claim(ClaimTypes.Role, user.Role)
            };

            // Check that claims are added correctly
            Console.WriteLine("Claims:");
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
                Expiration = tokenDescriptor.Expires.Value
            };
        }

    }
}