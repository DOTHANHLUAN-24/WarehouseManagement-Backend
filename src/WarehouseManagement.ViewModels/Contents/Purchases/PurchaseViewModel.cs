namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseViewModel : PurchaseBase
    {
        public int Id { get; set; }
        public string? SupplierName { get; set; }
        public string? ReceiptCode { get; set; }
        public string? ReferenceCode { get; set; }
        public string? Note { get; set; }
        public bool IsCanceled { get; set; }
        public string? NoteCancel { get; set; }
        public DateTime? CanceledDate { get; set; }
        public string? CanceledBy { get; set; }
        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
