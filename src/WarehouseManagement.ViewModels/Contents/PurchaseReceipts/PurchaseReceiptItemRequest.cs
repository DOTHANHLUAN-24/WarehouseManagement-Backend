namespace WarehouseManagement.ViewModels.Contents.PurchaseReceipts
{
    public class PurchaseReceiptItemRequest
    {
        // productId from frontend
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // unit cost (giá nhập)
        public decimal UnitCost { get; set; }
    }
}
