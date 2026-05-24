using System.Collections.Generic;

namespace WarehouseManagement.ViewModels.Contents.PurchaseReceipts
{
    public class PurchaseReceiptResponse
    {
        public int PurchaseId { get; set; }
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public string? SupplierName { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? ReceiptCode { get; set; }
        public string? ReferenceCode { get; set; }
        public string? Note { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int Status { get; set; }
        public bool IsCanceled { get; set; }
        public string? CancelReason { get; set; }
        public DateTime? CanceledDate { get; set; }
        public string? CanceledBy { get; set; }

        public List<PurchaseReceiptItemResponse> Items { get; set; } = new List<PurchaseReceiptItemResponse>();
    }
}
