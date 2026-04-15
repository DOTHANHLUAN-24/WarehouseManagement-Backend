using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Helpers;
using WarehouseManagement.BackendServer.Services.Interfaces;
using WarehouseManagement.ViewModels.Systems.AuditLogs;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController
    (
        ApplicationDbContext context, 
        IAuditLogService auditLogService,
        ILogger<AuditLogsController> logger
    ) : BaseController
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAuditLog() =>
            Ok(new ApiOkResponse<List<AuditLog>>(await auditLogService.GetAllAsync(), "Completed GetAllAuditLog API"));

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAuditLogByUserId(string userId)
        {
            var auditLogs = await context.AuditLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(auditLogs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuditLog(AuditLogCreateRequest auditLog)
        {
            try
            {
                await auditLogService.CreateAsync(auditLog);

                return Ok(new ApiOkResponse<string>("Created new auditLog"));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating audit log");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the audit log.");
            }
        }
    }
}
