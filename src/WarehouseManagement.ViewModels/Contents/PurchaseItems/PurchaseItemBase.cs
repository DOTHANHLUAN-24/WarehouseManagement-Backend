namespace WarehouseManagement.ViewModels.Contents.PurchaseItems
{
    public class PurchaseItemBase
    {
        public int PurchaseId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        // Optional warehouse location/bin for the item
        public string? WarehouseLocation { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
