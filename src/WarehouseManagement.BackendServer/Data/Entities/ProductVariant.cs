using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("ProductVariants")]
    public class ProductVariant : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SKU { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [Required]
        public ProductVariantStatus Status { get; set; } = ProductVariantStatus.Active;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
