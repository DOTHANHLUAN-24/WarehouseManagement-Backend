using System.ComponentModel.DataAnnotations;
using WarehouseManagement.ViewModels.Enums;

namespace WarehouseManagement.ViewModels.Contents.StockTransactions
{
    public class StockTransactionBase
    {
        public int ProductId { get; set; }

        public int ProductVariantId { get; set; }

        public int WarehouseId { get; set; }

        public int QuantityChange { get; set; }

        public StockTransactionType TransactionType { get; set; }

        public ReferenceType ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public int BalanceAfter { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string? Note { get; set; }

        public bool IsCanceled { get; set; } = false;

        public string? CancelReason { get; set; }

        public DateTime? CanceledDate { get; set; }

        public string? CanceledBy { get; set; }
    }
}
