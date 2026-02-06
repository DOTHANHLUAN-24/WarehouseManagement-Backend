using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Domain.Entities
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
        public DateOnly SnapshotDate { get; set; }

        public Product Product { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
    }
}
