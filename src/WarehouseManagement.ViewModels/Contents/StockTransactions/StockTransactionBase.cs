namespace WarehouseManagement.ViewModels.Contents.StockTransactions
{
    public class StockTransactionBase
    {
        public int ProductId { get; set; }

        public int WarehouseId { get; set; }

        public int QuantityChange { get; set; }

        public int? ReferenceId { get; set; }

        public int BalanceAfter { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
