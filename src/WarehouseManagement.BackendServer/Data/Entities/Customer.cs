using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Customers")]
    public class Customer : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        // JSON-friendly alias used by APIs. This is derived from the primary key to avoid
        // changing the database primary key shape (keeps compatibility with existing migrations).
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int CustomerId => Id;

        public CustomerStatus status = CustomerStatus.Active;

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [Required]
        public CustomerStatus Status { get; set; } = CustomerStatus.Active;

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    }
}