using System.Security.Claims;
using WarehouseManagement.BackendServer.Data.Entities;

namespace WarehouseManagement.BackendServer.Services.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<bool> CreateUser(User user, string password);

        Task<bool> CheckPassword(User user, string password);

        Task<bool> ChangePassword(User user, string currentPassword, string newPassword);

        Task<bool> ResetPassword(User user, string token, string newPassword);

        Task<List<Claim>> GetUserClaims(string email);

        Task<bool> AddClaim(User user, Claim claim);

        Task<bool> RemoveClaim(User user, Claim claim);

        Task<bool> LockUser(User user);

        Task<bool> UnlockUser(User user);
    }
}
