namespace WarehouseManagement.BackendServer.Repositories.Implements.Authentication
{
    public interface IRefreshTokenRepository
    {
        Task<int> Add(string userId, string token);

        Task<string?> GetUserIdByToken(string token);
        
        Task<bool> Validate(string token);
        
        Task<int> Update(string userId, string token);
    }
}
