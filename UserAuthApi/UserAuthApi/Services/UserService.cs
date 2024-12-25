using System;
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
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, AppDbContext context, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        // Get user by username
        public async Task<RegisterUserDto> GetUserByUsernameAsync(string username)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user by username.");
                throw new Exception("An error occurred while fetching user data.");
            }
        }

        // Get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users.");
                throw new Exception("An error occurred while fetching users data.");
            }
        }

        // Add a new user with password hashing
        public async Task AddUserAsync(User user, string plainPassword)
        {
            try
            {
                // Check if the user already exists
                var existingUser = await _userRepository.GetByUsernameAsync(user.UserName);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding new user.");
                throw new Exception("An error occurred while adding the user.");
            }
        }
    }
}