namespace WarehouseManagement.ViewModels.Contents.StockTransactions
{
    public class StockDocumentItemRequest
    {
        public int ProductId { get; set; }
        
        public int ProductVariantId { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public decimal TotalPrice { get; set; }
    }
}
