namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductViewModel : ProductBase
    {
        public int Id { get; set; }

        public bool IsDefault { get; set; } = false;

        public decimal SellingPrice { get; set; }

        public decimal OriginalPrice { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string? WarehouseLocation { get; set; }
    }
}
