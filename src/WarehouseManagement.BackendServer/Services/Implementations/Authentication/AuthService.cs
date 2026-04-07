using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Services.Interfaces.Authentication;

namespace WarehouseManagement.BackendServer.Services.Implementations.Authentication
{
    public class AuthService(UserManager<User> userManager) : IAuthService
    {
        public async Task<bool> AddClaim(User user, Claim claim)
        {
            var entity = await userManager.FindByEmailAsync(user.Email!);
            if (entity == null)
                return false;

            return (await userManager.AddClaimAsync(entity, claim)).Succeeded;
        }

        public async Task<bool> ChangePassword(User user, string currentPassword, string newPassword)
        {
            var entity = await userManager.FindByEmailAsync(user.Email!);
            if (entity == null)
                return false;

            if (currentPassword == newPassword)
                return false;

            return (await userManager.ChangePasswordAsync(entity, currentPassword, newPassword)).Succeeded;
        }

        public async Task<bool> CheckPassword(User user, string password)
        {
            var entity = await userManager.FindByEmailAsync(user.Email!);
            if (entity == null)
                return false;

            return await userManager.CheckPasswordAsync(entity, password);
        }

        public async Task<bool> CreateUser(User user, string password) =>
            (await userManager.CreateAsync(user, password)).Succeeded;


        public async Task<List<Claim>> GetUserClaims(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new List<Claim>();

            var claims = await userManager.GetClaimsAsync(user);
            return claims.ToList();
        }

        public async Task<bool> LockUser(User user)
        {
            var entity = await userManager.FindByEmailAsync(user.Email!);
            if (entity == null)
                return false;

            if (!user.IsActive)
                return false;

            entity.IsActive = false;

            return (await userManager.UpdateAsync(entity)).Succeeded;
        }

        public async Task<bool> RemoveClaim(User user, Claim claim)
        {
            var entity = await userManager.FindByEmailAsync(user.Email!);
            if (entity == null)
                return false;

            return (await userManager.RemoveClaimAsync(entity, claim)).Succeeded;
        }

        public async Task<bool> ResetPassword(User user, string token, string newPassword)
        {
            var entity = await userManager.FindByEmailAsync(user.Email!);
            if (entity == null)
                return false;

            return (await userManager.ResetPasswordAsync(entity, token, newPassword)).Succeeded;
        }

        public async Task<bool> UnlockUser(User user)
        {
            var entity = await userManager.FindByEmailAsync(user.Email!);
            if (entity == null)
                return false;
            if (user.IsActive)
                return false;

            entity.IsActive = true;

            return (await userManager.UpdateAsync(entity)).Succeeded;
        }

    }
}
