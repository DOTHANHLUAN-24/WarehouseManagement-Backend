namespace WarehouseManagement.ViewModels.Systems.AuditLogs
{
    public class AuditLogBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string Entity { get; set; }= string.Empty;

        public int EntityId { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
