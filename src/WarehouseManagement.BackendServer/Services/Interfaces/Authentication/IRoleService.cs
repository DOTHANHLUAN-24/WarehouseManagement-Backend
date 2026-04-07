using Microsoft.AspNetCore.Identity;
using WarehouseManagement.BackendServer.Data.Entities;

namespace WarehouseManagement.BackendServer.Services.Interfaces.Authentication
{
    public interface IRoleService
    {
        Task<string?> GetUserRole(string email);

        Task<bool> AddUserToRole(User user, string roleName);

        Task<List<string>> GetUserRoles(string email);

        Task<bool> AddUserToRoles(User user, List<string> roleNames);

        Task<bool> RemoveUserFromRole(User user, string roleName);

        Task<bool> IsUserInRole(User user, string roleName);

        Task<List<User>> GetUsersInRole(string roleName);

        Task<bool> CreateRoleAsync(IdentityRole role);

        Task<bool> RoleExistsAsync(string roleName);

        Task<bool> DeleteRole(string roleId);

        Task<List<IdentityRole>> GetAllRoles();
    }
}
