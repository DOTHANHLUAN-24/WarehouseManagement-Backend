using WarehouseManagement.ViewModels.Enums;

namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseBase
    {
        public int? SupplierId { get; set; }

        public int? CustomerId { get; set; }

        public bool IsExport { get; set; }

        public int Type { get; set; } // 1: Nhập, 2: Xuất

        public DateTime? PurchaseDate { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public PurchaseStatus Status { get; set; } = PurchaseStatus.None;
    }
}
