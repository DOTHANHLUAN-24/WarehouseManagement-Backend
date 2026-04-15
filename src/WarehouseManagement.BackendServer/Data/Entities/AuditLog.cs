using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Action { get; set; } = string.Empty;

        [Required]
        public string Entity { get; set; } = string.Empty;

        public string EntityId { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
