using System.Collections.Generic;

namespace WarehouseManagement.ViewModels.Contents.PurchaseReceipts
{
    public class PurchaseReceiptRequest
    {
        public int PurchaseId { get; set; } // client may send 0; server generates real id

        public int SupplierId { get; set; }

        public int WarehouseId { get; set; }

        public string? SupplierName { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string? ReferenceCode { get; set; }

        public string? Note { get; set; }

        public List<PurchaseReceiptItemRequest> Items { get; set; } = new List<PurchaseReceiptItemRequest>();
    }
}
