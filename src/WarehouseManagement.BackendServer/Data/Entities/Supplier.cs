using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Suppliers")]
    public class Supplier : ISoftDelete
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SupplierName { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
