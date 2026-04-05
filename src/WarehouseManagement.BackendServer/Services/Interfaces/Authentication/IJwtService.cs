using WarehouseManagement.ViewModels.Systems.Authentication;
using WarehouseManagement.ViewModels.Systems.Login;

namespace WarehouseManagement.BackendServer.Services.Interfaces
{
    public interface IJwtService
    {
        Task<LoginResponseModel?> Authenticate(LoginRequestModel request);
        
        Task<LoginResponseModel?> RefreshToken(string accessToken, string refreshToken);
        
        Task<bool> ChangePassword(string userName, ChangePasswordRequest request);
        
        Task RevokeToken(string userName);

        TimeSpan GetAccessTokenRemainingTime(string accessToken);

        Task<TimeSpan?> GetRefreshTokenRemainingTime(string userName);
    }
}
