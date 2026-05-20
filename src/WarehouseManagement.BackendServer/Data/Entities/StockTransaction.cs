using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("StockTransactions")]
    public class StockTransaction : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductVariantId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int QuantityChange { get; set; }

        [Required]
        public StockTransactionType TransactionType { get; set; }

        public string? Note { get; set; }

        [Required]
        public ReferenceType ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public int BalanceAfter { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsCanceled { get; set; } = false;

        [MaxLength(1000)]
        public string? CancelReason { get; set; }

        public DateTime? CanceledDate { get; set; }

        [MaxLength(200)]
        public string? CanceledBy { get; set; }
    }
}