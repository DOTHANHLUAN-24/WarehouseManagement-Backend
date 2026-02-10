namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductViewModel : ProductBase
    {
        public int Id { get; set; }

        public bool IsDefault { get; set; } = false;

        public string ImageUrl { get; set; } = string.Empty;
    }
}
