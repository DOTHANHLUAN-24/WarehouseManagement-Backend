using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Repositories.Interfaces;
using WarehouseManagement.BackendServer.Services.Interfaces;
using WarehouseManagement.ViewModels.Systems.AuditLogs;

namespace WarehouseManagement.BackendServer.Services.Implementations
{
    public class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
    {
        public Task CreateAsync(AuditLogCreateRequest request)
        {
            var auditLog = new AuditLog
            {
                UserId = request.UserId,
                Action = request.Action,
                Entity = request.Entity,
                EntityId = request.EntityId.ToString(),
                OldValue = request.OldValue,
                NewValue = request.NewValue
            };

            return auditLogRepository.CreateAuditLogAsync(auditLog);
        }

        public async Task<List<AuditLog>> GetAllAsync() =>
            await auditLogRepository.GetAllAsync();

        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to) =>
            await auditLogRepository.GetByDateRangeAsync(from, to);

        public async Task<List<AuditLog>> GetByEntityAsync(string entity) =>
            await auditLogRepository.GetByEntityAsync(entity);

        public async Task<List<AuditLog>> GetByUserAsync(string userId) =>
            await auditLogRepository.GetByUserIdAsync(userId);
    }
}
