namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductBase
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? CategoryId { get; set; }

        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
