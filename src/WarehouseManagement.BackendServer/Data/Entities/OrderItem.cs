using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("OrderItems")]
    public class OrderItem : IDateTracking
    {
        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        [Required]
        public int ProductVariantId { get; set; }
        
        public ProductVariant ProductVariant { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        public DateTime CreateDate { get; set; }
        
        public DateTime? LastModifiedDate { get; set; }
    }
}
