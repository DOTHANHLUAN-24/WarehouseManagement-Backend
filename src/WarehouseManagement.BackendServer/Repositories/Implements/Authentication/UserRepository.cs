using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Repositories.Interfaces.Authentication;

namespace WarehouseManagement.BackendServer.Repositories.Implements.Authentication
{
    /// <summary>
    /// Repository handling user persistence and identity operations.
    /// Uses UserManager for Identity-safe operations (password hashing, deletion).
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserRepository> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User?> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<User?> CreateUser(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to create user {UserName}: {Errors}", user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                return null;
            }

            // Assign default "User" role if exists
            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return user;
        }

        public async Task<bool> UpdateUser(string userId, User user)
        {
            var existing = await _userManager.FindByIdAsync(userId);
            if (existing == null) return false;

            // Update allowed fields
            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.PhoneNumber = user.PhoneNumber;
            existing.Email = user.Email;
            existing.UserName = user.UserName;
            existing.IsActive = user.IsActive;

            var result = await _userManager.UpdateAsync(existing);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to update user {UserId}: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            var existing = await _userManager.FindByIdAsync(userId);
            if (existing == null) return false;

            var result = await _userManager.DeleteAsync(existing);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to delete user {UserId}: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            return true;
        }
    }
}