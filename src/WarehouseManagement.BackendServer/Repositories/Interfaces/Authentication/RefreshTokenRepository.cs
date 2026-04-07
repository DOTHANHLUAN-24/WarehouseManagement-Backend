using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Repositories.Implements.Authentication;

namespace WarehouseManagement.BackendServer.Repositories.Interfaces.Authentication
{
    public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
    {
        public async Task<int> Add(string userId, string token)
        {
            context.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                Token = token
            });

            return await context.SaveChangesAsync();
        }

        public async Task<string?> GetUserIdByToken(string token)
        {
            return (await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token))?.UserId;
        }

        public async Task<bool> Validate(string token)
        {
            return await context.RefreshTokens
                .AnyAsync(x => x.Token == token);
        }

        public async Task<int> Update(string userId, string token)
        {
            var entity = await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (entity == null) return -1;

            entity.Token = token;
            return await context.SaveChangesAsync();
        }
    }
}
