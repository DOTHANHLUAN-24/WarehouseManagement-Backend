namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductDetailViewModel : ProductViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int? Rating { get; set; } = 0;

        public int? ParentId { get; set; }

        public bool IsApproved { get; set; } = true;
    }
}
