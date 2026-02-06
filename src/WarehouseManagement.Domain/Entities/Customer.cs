using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Domain.Entities
{
    public class Customer : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
