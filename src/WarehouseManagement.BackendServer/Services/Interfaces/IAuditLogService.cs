using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Systems.AuditLogs;

namespace WarehouseManagement.BackendServer.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task CreateAsync(AuditLogCreateRequest request);

        Task<List<AuditLog>> GetAllAsync();

        Task<List<AuditLog>> GetByUserAsync(string userId);

        Task<List<AuditLog>> GetByEntityAsync(string entity);

        Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}
