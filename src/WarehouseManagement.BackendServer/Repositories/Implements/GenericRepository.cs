using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Repositories.Interfaces;

namespace WarehouseManagement.BackendServer.Repositories.Implements
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(TKey id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity is null) return false;

            _context.Set<TEntity>().Remove(entity);
            return true;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
