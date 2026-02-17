using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    public class StockSnapshot
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime SnapshotDate { get; set; }
    }
}
