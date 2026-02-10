using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Categories")]
    public class Category : IDateTracking, ISoftDelete
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string SeoAlias { get; set; } = string.Empty;

        [MaxLength(500)]
        public string SeoDescription { get; set; } = string.Empty;

        [Required]
        public int SortOrder { get; set; }

        public int? ParentId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
