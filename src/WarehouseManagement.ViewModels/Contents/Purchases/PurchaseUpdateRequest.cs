using System;
using System.Collections.Generic;
using WarehouseManagement.ViewModels.Contents.PurchaseItems;

namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseUpdateRequest : PurchaseBase
    {
        public int WarehouseId { get; set; }

        public string? SupplierName { get; set; }

        public string? CustomerName { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string? ReferenceCode { get; set; }

        public string? Note { get; set; }

        public decimal TotalAmount { get; set; } // Map vào TotalCost của Entity

        public List<PurchaseItemUpdateRequest> Items { get; set; } = new List<PurchaseItemUpdateRequest>();
    }
}
