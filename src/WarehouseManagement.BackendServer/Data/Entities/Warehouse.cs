using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Warehouses")]
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;
        
        [Required]
        public int Capacity { get; set; }
        
        [Required]
        public string Email { get; set; } = null!;

        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
