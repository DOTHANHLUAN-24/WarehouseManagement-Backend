using System.Security.Claims;

namespace WarehouseManagement.BackendServer.Services.Interfaces.Authentication
{
    public interface ITokenService
    {
        string GenerateAccessToken(List<Claim> claims);

        string GenerateRefreshToken();
        
        Task<bool> ValidateRefreshToken(string token);
        
        Task<string?> GetUserIdFromRefreshToken(string token);
    }
}
