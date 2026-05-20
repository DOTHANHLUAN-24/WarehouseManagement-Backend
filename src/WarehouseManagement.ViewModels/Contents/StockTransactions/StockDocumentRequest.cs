using WarehouseManagement.ViewModels.Enums;

namespace WarehouseManagement.ViewModels.Contents.StockTransactions
{
    public class StockDocumentRequest
    {
        public StockTransactionType TransactionType { get; set; }

        public int WarehouseId { get; set; }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; } = string.Empty;
        
        public DateTime ReceiptDate { get; set; }
        
        public string ReferenceCode { get; set; } = string.Empty;
        
        public string Note { get; set; } = string.Empty;
        
        public decimal TotalAmount { get; set; }

        public List<StockDocumentItemRequest> Items { get; set; } = new();
    }
}
