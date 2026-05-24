namespace WarehouseManagement.ViewModels.Contents.PurchaseReceipts
{
    public class PurchaseReceiptItemResponse
    {
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
