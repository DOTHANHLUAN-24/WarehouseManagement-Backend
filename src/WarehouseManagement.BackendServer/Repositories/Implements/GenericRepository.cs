using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Repositories.Interfaces;

namespace WarehouseManagement.BackendServer.Repositories.Implements
{
    public class GenericRepository<TEntity, TKey>(ApplicationDbContext context) : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        public async Task AddAsync(TEntity entity)
        {
            await context.Set<TEntity>().AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(TKey id)
        {
            var entity = await context.Set<TEntity>().FindAsync(id);
            if (entity is null) return false;

            context.Set<TEntity>().Remove(entity);
            return true;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await context.Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        public Task UpdateAsync(TEntity entity)
        {
            context.Set<TEntity>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
