using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("ProductComments")]
    public class ProductComment : IDateTracking, ISoftDelete
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int? ProductVariantId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]

        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Range(1, 5)]
        public int? Rating { get; set; }

        public int? ParentId { get; set; }

        public bool IsApproved { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public ICollection<ProductComment> Replies { get; set; }
            = new List<ProductComment>();
    }
}