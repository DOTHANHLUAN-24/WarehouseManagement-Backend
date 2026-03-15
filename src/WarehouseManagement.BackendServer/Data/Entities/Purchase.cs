using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Purchases")]
    public class Purchase : IDateTracking, ISoftDelete
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SupplierId { get; set; }

        public DateTime? PurchaseDate { get; set; }

        [Precision(18, 2)]
        public decimal TotalCost { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
