namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseBase
    {
        public int SupplierId { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
