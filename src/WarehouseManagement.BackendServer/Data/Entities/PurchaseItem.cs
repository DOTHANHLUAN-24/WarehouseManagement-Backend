using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("PurchaseItems")]
    public class PurchaseItem : IDateTracking
    {
        public int PurchaseId { get; set; }

        public Purchase Purchase { get; set; } = null!;

        public int ProductId { get; set; }
        
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
