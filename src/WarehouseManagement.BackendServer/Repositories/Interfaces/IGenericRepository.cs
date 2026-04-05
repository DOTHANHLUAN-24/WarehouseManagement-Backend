namespace WarehouseManagement.BackendServer.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity?> GetByIdAsync(TKey id);
        
        Task AddAsync(TEntity entity);
        
        Task UpdateAsync(TEntity entity);
        
        Task<bool> DeleteAsync(TKey id);
    }
}
