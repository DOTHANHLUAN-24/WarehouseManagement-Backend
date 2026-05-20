namespace WarehouseManagement.ViewModels.Contents.PurchaseItems
{
    public class PurchaseItemCreateRequest : PurchaseItemBase
    {
        public int ProductId { get; set; } // Dùng cho StockTransaction

        public decimal UnitPrice { get; set; } // Map vào UnitCost của Entity

        public decimal TotalPrice { get; set; } // Không lưu DB, nhưng có thể dùng để validate
    }
}
