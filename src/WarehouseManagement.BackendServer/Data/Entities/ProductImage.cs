using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("ProductImages")]
    public class ProductImage : IDateTracking, ISoftDelete
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public Product Product { get; set; } = null!;

        public int SortOrder { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

}
