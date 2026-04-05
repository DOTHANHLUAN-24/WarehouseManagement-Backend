using System.Security.Claims;
using WarehouseManagement.BackendServer.Repositories.Interfaces.Authentication;

namespace WarehouseManagement.BackendServer.Repositories.Implements.Authentication
{
    public class TokenManagement : ITokenManagement
    {
        public Task<int> AddRefreshToken(string userId, string refreshToken)
        {
            throw new NotImplementedException();
        }

        public string GenerateToken(List<Claim> claims)
        {
            throw new NotImplementedException();
        }

        public string GetRefreshToken()
        {
            throw new NotImplementedException();
        }

        public List<Claim> GetUserClaimsFromToken(string token)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdByRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateRefreshToken(string userId, string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
