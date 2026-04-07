using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Services.Interfaces.Authentication;

namespace WarehouseManagement.BackendServer.Services.Implementations.Authentication
{
    public class RoleService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : IRoleService
    {
        public async Task<bool> AddUserToRole(User user, string roleName) =>
            (await userManager.AddToRoleAsync(user, roleName)).Succeeded;

        public async Task<bool> AddUserToRoles(User user, List<string> roleNames) =>
            (await userManager.AddToRolesAsync(user, roleNames)).Succeeded;

        public async Task<bool> CreateRoleAsync(IdentityRole role) =>
            (await roleManager.CreateAsync(role)).Succeeded;

        public async Task<bool> DeleteRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
                return false;

            return (await roleManager.DeleteAsync(role)).Succeeded;
        }

        public async Task<List<IdentityRole>> GetAllRoles() => 
            await roleManager.Roles.ToListAsync();

        public async Task<string?> GetUserRole(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var roles = await userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }

        public async Task<List<string>> GetUserRoles(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new List<string>();

            var roles = await userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<List<User>> GetUsersInRole(string roleName) =>
            await userManager.GetUsersInRoleAsync(roleName)
                as List<User>
                ?? new List<User>();

        public async Task<bool> IsUserInRole(User user, string roleName) =>
            await userManager.IsInRoleAsync(user, roleName);

        public async Task<bool> RemoveUserFromRole(User user, string roleName)
        {
            if (user == null || string.IsNullOrWhiteSpace(roleName))
                return false;

            var isInRole = await userManager.IsInRoleAsync(user, roleName);
            if (!isInRole)
                return false;

            var result = await userManager.RemoveFromRoleAsync(user, roleName);

            return result.Succeeded;
        }

        public async Task<bool> RoleExistsAsync(string roleName) =>
            await roleManager.RoleExistsAsync(roleName);
    }
}
