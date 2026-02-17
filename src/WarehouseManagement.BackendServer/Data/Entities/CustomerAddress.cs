using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("CustomerAddresses")]
    public class CustomerAddress : ISoftDelete
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(200)]
        public string AddressLine { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
