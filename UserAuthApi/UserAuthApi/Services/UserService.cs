using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserAuthApi.Data;
using UserAuthApi.dto;
using UserAuthApi.Models;
using UserAuthApi.Repositories;

namespace UserAuthApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public UserService(IUserRepository userRepository, AppDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        // Get user by username
        public async Task<RegisterUserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return null;

            return new RegisterUserDto
            {
                Username = user.UserName,
                Role = user.Role
            };
        }



        // Get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        // Add a new user with password hashing
        public async Task AddUserAsync(User user, string plainPassword)
        {
            // Check if the user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (existingUser != null)
            {
                throw new Exception("User already exists.");
            }

            // Use Identity's PasswordHasher to hash the plain password
            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(user, plainPassword);  // Hash the plain password

            user.PasswordHash = hashedPassword;

            // Add the user to the repository
            await _userRepository.AddAsync(user);
        }
    }
}
