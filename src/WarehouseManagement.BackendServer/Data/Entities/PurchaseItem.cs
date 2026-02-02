using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("PurchaseItems")]
    public class PurchaseItem : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        public int PurchaseId { get; set; }

        public Purchase Purchase { get; set; } = null!;

        public int ProductId { get; set; }
        
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal CostPrice { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
