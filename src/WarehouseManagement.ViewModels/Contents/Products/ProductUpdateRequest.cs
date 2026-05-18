namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductUpdateRequest : ProductBase
    {
        public decimal SellingPrice { get; set; }

        public decimal OriginalPrice { get; set; }

        public int InitialStock { get; set; }
        
        public string? SKU { get; set; } // IMEI
    }
}
