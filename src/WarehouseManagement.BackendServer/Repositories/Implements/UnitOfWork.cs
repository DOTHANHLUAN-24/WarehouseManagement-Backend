using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Repositories.Interfaces;

namespace WarehouseManagement.BackendServer.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
