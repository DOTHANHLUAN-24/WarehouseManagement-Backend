using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string Action { get; set; } = null!;

        [Required]
        public string Entity { get; set; } = null!;

        public Guid? EntityId { get; set; }

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
