using WarehouseManagement.BackendServer.Data.Entities;

namespace WarehouseManagement.BackendServer.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<bool> CreateAuditLogAsync(AuditLog auditLog);

        Task<List<AuditLog>> GetAllAsync();

        Task<List<AuditLog>> GetByUserIdAsync(string userId);

        Task<List<AuditLog>> GetByEntityAsync(string entity);

        Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}
