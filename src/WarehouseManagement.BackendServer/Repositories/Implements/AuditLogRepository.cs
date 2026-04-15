using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Repositories.Interfaces;

namespace WarehouseManagement.BackendServer.Repositories.Implements
{
    public class AuditLogRepository(ApplicationDbContext context) : IAuditLogRepository
    {
        public async Task<bool> CreateAuditLogAsync(AuditLog auditLog)
        {
            await context.AuditLogs.AddAsync(auditLog);

            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<AuditLog>> GetAllAsync() =>
            await context.AuditLogs.ToListAsync();

        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to) =>
            (await context.AuditLogs.Where(x => x.CreatedAt >= from && x.CreatedAt <= to).ToListAsync());

        public async Task<List<AuditLog>> GetByEntityAsync(string entity)=>
            await context.AuditLogs.Where(x => x.Entity == entity).ToListAsync();

        public async Task<List<AuditLog>> GetByUserIdAsync(string userId)=> 
            await context.AuditLogs.Where(x => x.UserId == userId).ToListAsync();
    }
}
