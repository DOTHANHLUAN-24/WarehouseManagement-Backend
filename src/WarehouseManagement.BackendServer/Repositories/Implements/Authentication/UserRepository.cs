using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Repositories.Interfaces.Authentication;

namespace WarehouseManagement.BackendServer.Repositories.Implements.Authentication
{
    public class UserRepository(UserManager<User> userManager) : IUserRepository
    {
        public async Task<bool> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return (await userManager.DeleteAsync(user)).Succeeded;
        }

        public async Task<List<User>> GetAllUsers() =>
            await userManager.Users.ToListAsync();

        public async Task<User?> GetUserByEmail(string email) =>
            await userManager.FindByEmailAsync(email);

        public async Task<User?> GetUserById(string id) =>
            await userManager.FindByIdAsync(id)!;

        public async Task<bool> UpdateUser(string userId, User user)
        {
            var entity = await userManager.FindByIdAsync(userId);
            if (entity == null)
                return false;

            entity.FirstName = user.FirstName;
            entity.LastName = user.LastName;
            entity.PhoneNumber = user.PhoneNumber;

            return (await userManager.UpdateAsync(entity)).Succeeded;
        }
    }
}
