using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Repositories.Interfaces;

namespace WarehouseManagement.BackendServer.Repositories.Implements
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}
