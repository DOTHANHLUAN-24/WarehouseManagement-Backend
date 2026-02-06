using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Domain.Entities
{
    public class ProductComment : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

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

        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey(nameof(ParentId))]
        public ProductComment? Parent { get; set; }

        public ICollection<ProductComment> Replies { get; set; } = new List<ProductComment>();
    }
}
