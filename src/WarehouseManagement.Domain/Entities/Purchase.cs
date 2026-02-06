using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Domain.Entities
{
    public class Purchase : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SupplierId { get; set; }

        public Supplier Supplier { get; set; } = null!;

        public DateTime? PurchaseDate { get; set; }

        [Precision(18, 2)]
        public decimal TotalCost { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
