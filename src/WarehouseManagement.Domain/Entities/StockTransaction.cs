using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Entities
{
    public class StockTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required]
        public int WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; } = null!;

        [Required]
        public int QuantityChange { get; set; }

        [Required]
        public StockTransactionType TransactionType { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public int BalanceAfter { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
