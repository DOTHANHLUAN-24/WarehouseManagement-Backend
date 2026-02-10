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

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string UserId { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [Required]
        public CustomerStatus Status { get; set; } = CustomerStatus.Active;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        
        public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    }
}
