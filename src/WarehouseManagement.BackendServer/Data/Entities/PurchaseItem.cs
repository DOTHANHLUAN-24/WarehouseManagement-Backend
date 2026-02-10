using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("PurchaseItems")]
    public class PurchaseItem : IDateTracking
    {

        public int PurchaseId { get; set; }

        public Purchase Purchase { get; set; } = null!;

        [Required]
        public int ProductVariantId { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal UnitCost { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }

}
