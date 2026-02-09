namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductBase
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
