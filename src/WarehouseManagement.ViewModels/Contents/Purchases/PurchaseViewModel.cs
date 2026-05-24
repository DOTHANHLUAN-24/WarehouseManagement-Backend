namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseViewModel : PurchaseBase
    {
        public int Id { get; set; }
        public string? SupplierName { get; set; }
        public string? ReceiptCode { get; set; }
        public string? ReferenceCode { get; set; }
        public string? Note { get; set; }
    }
}
