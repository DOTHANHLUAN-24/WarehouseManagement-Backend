using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("OrderItems")]
    public class OrderItem : IDateTracking
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
