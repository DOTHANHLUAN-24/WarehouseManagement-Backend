namespace WarehouseManagement.ViewModels.Contents
{
    public class LowStockItemViewModel
    {
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        public string? ProductName { get; set; }
        public string? VariantName { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public int WarehouseId { get; set; } // if per-warehouse; 0 if aggregated
    }
}