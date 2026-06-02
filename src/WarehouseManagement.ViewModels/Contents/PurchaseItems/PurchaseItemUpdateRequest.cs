namespace WarehouseManagement.ViewModels.Contents.PurchaseItems
{
    public class PurchaseItemUpdateRequest : PurchaseItemBase
    {
        public int ProductId { get; set; } // Dùng cho StockTransaction

        public decimal TotalPrice { get; set; } // Không lưu DB, nhưng có thể dùng để validate
    }
}
