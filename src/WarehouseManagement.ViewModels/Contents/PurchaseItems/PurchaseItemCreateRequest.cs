namespace WarehouseManagement.ViewModels.Contents.PurchaseItems
{
    public class PurchaseItemCreateRequest : PurchaseItemBase
    {
        public int ProductId { get; set; } // Dùng cho StockTransaction

        // Use UnitCost inherited from PurchaseItemBase as the input price for this item (giá nhập)

        public decimal TotalPrice { get; set; } // Không lưu DB, nhưng có thể dùng để validate
    }
}
