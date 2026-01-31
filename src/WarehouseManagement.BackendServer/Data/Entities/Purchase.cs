using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Purchases")]
    public class Purchase : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SupplierId { get; set; }
        
        public Supplier Supplier { get; set; } = null!;

        public DateTime? PurchaseDate { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
