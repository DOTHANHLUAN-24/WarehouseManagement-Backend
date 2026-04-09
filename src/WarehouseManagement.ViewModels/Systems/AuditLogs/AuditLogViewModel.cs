namespace WarehouseManagement.ViewModels.Systems.AuditLogs
{
    public class AuditLogViewModel : AuditLogBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
